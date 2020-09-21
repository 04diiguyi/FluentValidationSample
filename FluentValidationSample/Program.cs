using System;
using System.Net;
using FluentValidation;

namespace FluentValidationSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var person = new Person();
            person.Name = "This is a super long name";
            person.DateTime = DateTime.UtcNow.AddMinutes(1);

            var personValidator = new PersonValidator();
            var result = personValidator.Validate(person);

            Console.WriteLine(result);
        }
    }

	public class Person
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public int Age { get; set; }
        public DateTime DateTime { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }

	public class PersonValidator : AbstractValidator<Person>
	{
		public PersonValidator()
		{
			RuleFor(x => x.Id).NotNull().WithMessage("Id is empty");
			RuleFor(x => x.Name).Length(1, 10);
			RuleFor(x => x.Email).EmailAddress();
			RuleFor(x => x.Age).InclusiveBetween(18, 60);

            RuleFor(x => x.DateTime).Must(MustNotInFuture).WithMessage("Invalid time, it is in future.");

            RuleFor(x => x.StatusCode).NotEmpty().WithMessage("Status code is empty.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.ErrorMessage).NotEmpty().When(x => !IsSuccess(x.StatusCode)).WithMessage("Error message is empty.");
                });
        }

        private bool MustNotInFuture(DateTime value)
        {
            return value.CompareTo(DateTime.UtcNow) < 0;
        }

        private bool IsSuccess(HttpStatusCode statusCode)
        {
            return (int)statusCode >= 200 && (int)statusCode <= 299;
        }
    }
}
