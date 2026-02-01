using BlazingQuiz.Api.Data.Entities;
using BlazingQuiz.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazingQuiz.Api.Data;

public class QuizContext : DbContext
{
    private readonly IPasswordHasher<User> passwordHasher;

    public QuizContext(DbContextOptions<QuizContext> options, IPasswordHasher<User> passwordHasher) : base(options)
    {
        this.passwordHasher = passwordHasher;
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<StudentQuiz> StudentQuizzes { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var adminUser = new User
        {
            Id = 1,
            Name = "sandy",
            Email = "admin@gmail.com",
            Phone = "1234567890",
            Role = nameof(UserRole.Admin),
            IsApproved = true,            
            PasswordHash = "AQAAAAIAAYagAAAAEEV2REw3WgkWkwuW2XKSoekgeYFPUL/ScWATuz5JLqzzipc4ClGDCb66O80OceqovA==",
        };
        // adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "123456");

        modelBuilder.Entity<User>().HasData(adminUser);
    }
}
