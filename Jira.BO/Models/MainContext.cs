using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfm = System.Configuration.ConfigurationManager;

namespace Jira.BO.Models {
    public partial class MainContext : DbContext {

        /// <summary>
        /// Constructor, defining database name
        /// </summary>
        public MainContext() : base("JiraDatabase") { }


        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  
        /// This override checks to see if database needs to be created or dropped (if schema changed.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.
        /// </remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();            
            base.OnModelCreating(modelBuilder);
        }        


        // Database items 
        public DbSet<Project> Projects { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserItemWorklog> UserItemWorklogs { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }




        /// <summary>
        /// Initializer class that after creating/altering datebase will have an initial script to run
        /// making sure if any required data is added.
        /// </summary>
        public class Initializer : IDatabaseInitializer<MainContext> {
            public void InitializeDatabase(MainContext context) {
                if (context.Database.Exists() && !context.Database.CompatibleWithModel(false)) {
                    context.Database.Delete();
                }
                if (!context.Database.Exists()) {
                    context.Database.Create();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(@" SET IDENTITY_INSERT [dbo].[Users] ON ");
                    sb.Append(@" MERGE [dbo].[Users] AS T ");
                    sb.Append(@" USING ( ");
                    sb.Append(@" VALUES ");
                    sb.Append(@" (0, N'unassigned', '', 'Unassigned', '', getdate(), getdate()) ");
                    sb.Append(@" ) as S ([UserID], [Name], [EmailAddress], [DisplayName], [SelfUrl], [CreateDt], [ModifyDt]) ");
                    sb.Append(@" ON T.[UserID] = S.[UserID] ");
                    sb.Append(@" WHEN NOT MATCHED THEN ");
                    sb.Append(@" Insert ([UserID], [Name], [EmailAddress], [DisplayName], [SelfUrl], [CreateDt], [ModifyDt]) ");
                    sb.Append(@" VALUES (S.[UserID], S.[Name], S.[EmailAddress], S.[DisplayName], S.[SelfUrl], S.[CreateDt], S.[ModifyDt]); ");
                    sb.Append(@" SET IDENTITY_INSERT [dbo].[Users] OFF ");

                    context.Database.ExecuteSqlCommand(sb.ToString());

                }
            }
        }
        
    }


    





}
