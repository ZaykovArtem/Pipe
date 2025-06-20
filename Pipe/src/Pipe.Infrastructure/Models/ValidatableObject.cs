﻿using System.ComponentModel.DataAnnotations;


namespace Pipe.Infrastructure.Models
{
	public abstract class ValidatableObject
	{
		public virtual bool IsValid()
		{
			return Validate().Count == 0;
		}

		public virtual IList<ValidationResult> Validate()
		{
			IList<ValidationResult> validationResults = new List<ValidationResult>();
			Validator.TryValidateObject(this, new ValidationContext(this, null, null), validationResults, true);
			return validationResults;
		}
	}
}
