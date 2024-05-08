using BitirmeProjesi.Data.Entities.JsonEntities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeProjesi.Business.Validators;

// FluentValidation kütüphanesini kullanarak AppUser sınıfı için bir doğrulama (validation) sınıfı oluşturur.
public class AppUserValidator : AbstractValidator<AppUser>
{
    public AppUserValidator()
    {
        RuleFor(a => a.Adres)
            .NotEmpty()
            .WithMessage("Adres alanı boş bırakılamaz")
            .MinimumLength(10)
            .WithMessage("Açık adresiniz 10 karakterden az olamaz")
            .MaximumLength(400)
            .WithMessage("Açık adresiniz 400 karakterden fazla olamaz");

        RuleFor(a => a.City)
            .NotEmpty()
            .WithMessage("Şehir alanı boş bırakılamaz")
            .MaximumLength(50)
            .WithMessage("Şehir alanı çok uzun");

        RuleFor(a => a.Country)
           .NotEmpty()
           .WithMessage("Ülke alanı boş bırakılamaz")
           .MaximumLength(50)
           .WithMessage("Ülke alanı çok uzun");

        RuleFor(a => a.UserName)
          .NotEmpty()
          .WithMessage("Kullanıcı adı boş bırakılamaz")
          .MinimumLength(3)
          .WithMessage("Kullanıcı adınız 3 karakterden az olamaz")
          .MaximumLength(40)
          .WithMessage("Kullanıcı adınız 40 karakterden fazla olamaz");

        RuleFor(a => a.Email)
           .NotEmpty()
           .WithMessage("Email alanı boş bırakılamaz")
           .MinimumLength(10)
           .WithMessage("Email alanı 10 karakterden az olamaz")
           .MaximumLength(40)
           .WithMessage("Email alanı 40 karakterden fazla olamaz");

        RuleFor(a => a.PhoneNumber)
          .NotEmpty()
          .WithMessage("Telefon numarası boş bırakılamaz")
          .MinimumLength(10)
          .WithMessage("Telefon numarası 10 karakterden az olamaz")
          .MaximumLength(40)
          .WithMessage("Telefon numarası 40 karakterden fazla olamaz");
    }
}
