namespace Azoxia.AdaIsAkademi.Application.Tests
{
    using Azoxia.AdaIsAkademi.Application.Tests.Support;
    using Azoxia.AdaIsAkademi.Domain;
    using Azoxia.AdaIsAkademi.Persistence;
    using Azoxia.Core.Application;
    using Azoxia.Core.Application.Commands;
    using Azoxia.Core.Exceptions;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Handler tests for worker profile update command.
    /// </summary>
    public sealed class WorkerProfileCommandHandlersTests
    {
        #region Methods

        [Fact]
        public async Task UpdateWorkerProfileHandler_updates_nationality_and_university_for_actor_worker()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;
            AdaIsAkademiDbContext db = sp.GetRequiredService<AdaIsAkademiDbContext>();

            Worker worker = await SeedWorkerAsync(db);
            executionContext.ReplaceClaim("worker_id", worker.Id.ToString());

            var handler = new UpdateWorkerProfileCommandHandler(sp);
            await ((IRequestHandler<UpdateWorkerProfileCommand, Unit>)handler).HandleAsync(
                new UpdateWorkerProfileCommand
                {
                    Nationality = "TR",
                    University = "Bogazici University",
                },
                CancellationToken.None);

            Worker? reloaded = await db.Set<Worker>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == worker.Id);
            reloaded.Should().NotBeNull();
            reloaded!.Nationality.Should().Be("TR");
            reloaded.University.Should().Be("Bogazici University");
        }

        [Fact]
        public async Task UpdateWorkerProfileHandler_throws_when_actor_worker_not_found()
        {
            var executionContext = new TestExecutionContext(isAuthenticated: true);
            executionContext.ReplaceClaim("worker_id", "999999");

            using ServiceProvider root = ApplicationHandlerTestServices.CreateProvider(executionContext);
            using IServiceScope scope = root.CreateScope();
            IServiceProvider sp = scope.ServiceProvider;

            var handler = new UpdateWorkerProfileCommandHandler(sp);
            Func<Task> act = async () =>
                await ((IRequestHandler<UpdateWorkerProfileCommand, Unit>)handler).HandleAsync(
                    new UpdateWorkerProfileCommand
                    {
                        Nationality = "TR",
                        University = "Any University",
                    },
                    CancellationToken.None);

            AzoxiaException ex = (await act.Should().ThrowAsync<AzoxiaException>()).Which;
            ex.Error.Should().Be(AzoxiaErrorCodes.NotFound);
        }

        #endregion Methods

        #region Utils

        private static async Task<Worker> SeedWorkerAsync(AdaIsAkademiDbContext db)
        {
            var user = new SystemUser("worker-profile@test.local", "Password1!", SystemUserType.Worker);
            db.Set<SystemUser>().Add(user);
            await db.SaveChangesAsync();

            Worker worker = new(user.Id);
            db.Set<Worker>().Add(worker);
            await db.SaveChangesAsync();
            return worker;
        }

        #endregion Utils
    }
}
