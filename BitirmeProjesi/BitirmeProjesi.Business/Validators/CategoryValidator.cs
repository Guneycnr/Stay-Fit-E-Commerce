using BitirmeProjesi.Data.Entities.JsonEntities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeProjesi.Business.Validators;

// FluentValidation kütüphanesini kullanarak Category sınıfı için bir doğrulama (validation) sınıfı oluşturur.
public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.CategoryName)
           .NotEmpty()
           .WithMessage("Kategori adı bırakılamaz")
           .MinimumLength(3)
           .WithMessage("Kategori adı 3 karakterden az olamaz")
           .MaximumLength(20)
           .WithMessage("Kategori adı 20 karakterden fazla olamaz");

    }
}
