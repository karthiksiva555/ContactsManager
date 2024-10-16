using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactsManager.Application.Helpers;

public static class ValidationHelper
{
    public static void Validate(Object obj)
    {
        ValidationContext validationContext = new(obj);
        List<ValidationResult> validationResults = [];
        var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
        if(!isValid)
        {
            // Get all the error messages
            var errorMessages = from result in validationResults select result.ErrorMessage;
            throw new ArgumentException(string.Join(",", errorMessages));
        }
    }
}