using BitirmeProjesi.Data.Entities.JsonEntities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeProjesi.Business.Validators;

// FluentValidation kütüphanesini kullanarak Contact sınıfı için bir doğrulama (validation) sınıfı oluşturur.
public class ContactValidator : AbstractValidator<Contact>
{
    public ContactValidator()
    {
        RuleFor(a => a.Name)
           .NotEmpty()
           .WithMessage("İsim alanı boş bırakılamaz")
           .MinimumLength(3)
           .WithMessage("İsim alanı 3 karakterden az olamaz")
           .MaximumLength(50)
           .WithMessage("İsim alanı 50 karakterden fazla olamaz");

        RuleFor(a => a.Email)
            .NotEmpty()
            .WithMessage("Email alanı boş bırakılamaz")
            .MinimumLength(10)
            .WithMessage("Email alanı 10 karakterden az olamaz")
            .MaximumLength(50)
            .WithMessage("Email alanı 50 karakterden fazla olamaz");

        RuleFor(a => a.Message)
           .NotEmpty()
           .WithMessage("Mesaj alanı boş bırakılamaz")
           .MinimumLength(10)
           .WithMessage("Mesaj alanı 10 karakterden az olamaz")
           .MaximumLength(800)
           .WithMessage("Email alanı 800 karakterden fazla olamaz");
    }
}
