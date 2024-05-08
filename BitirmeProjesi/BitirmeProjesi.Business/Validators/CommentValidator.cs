using BitirmeProjesi.Data.Entities.JsonEntities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeProjesi.Business.Validators;

// FluentValidation kütüphanesini kullanarak Comment sınıfı için bir doğrulama (validation) sınıfı oluşturur.
public class CommentValidator : AbstractValidator<Comment>
{
    public CommentValidator()
    {
        RuleFor(a => a.CommentLine)
           .NotEmpty()
           .WithMessage("Yorum satırı boş bırakılamaz")
           .MinimumLength(4)
           .WithMessage("Yorum satırı 4 karakterden az olamaz")
           .MaximumLength(500)
           .WithMessage("Yorum satırı 500 karakterden fazla olamaz");

    }
}
