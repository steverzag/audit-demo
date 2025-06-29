using FluentValidation;
using UsersTasks.API.DTOs;

namespace UsersTasks.API.Validation
{
	public class CreateTaskValidator : AbstractValidator<CreateTaskRequest>
	{
		public CreateTaskValidator()
		{
			RuleFor(t => t.Title)
				.Length(2, 100)
				.WithMessage("Title must be between 2 and 100 characters long");
			RuleFor(t => t.Status)
				.IsInEnum()
				.WithMessage("Invalid status value");
			RuleFor(t => t.Priority)
				.IsInEnum()
				.WithMessage("Invalid priority value");
		}
	}

	public class UpdateTaskValidator : AbstractValidator<UpdateTaskRequest>
	{
		public UpdateTaskValidator()
		{
			RuleFor(t => t.Title)
				.Length(2, 100)
				.WithMessage("Title must be between 2 and 100 characters long");
			RuleFor(t => t.Status)
				.IsInEnum()
				.WithMessage("Invalid status value");
			RuleFor(t => t.Priority)
				.IsInEnum()
				.WithMessage("Invalid status value");
		}
	}
}
