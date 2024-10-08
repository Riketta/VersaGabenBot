using VersaGabenBot.Data.Models;
using VersaGabenBot.Data.Repositories;
using VersaGabenBot.Tests.Mocks;

namespace VersaGabenBot.Tests.Repositories
{
    [TestClass]
    public class GuildRepositoryTests
    {
        private readonly MockDatabaseContext _db = new MockDatabaseContext();
        private GuildRepository _guildRepository;

        [TestInitialize]
        public void Setup()
        {
            _db.Wipe();
            new DatabaseInitializer(_db).Initialize();
            _guildRepository = new GuildRepository(_db);
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        [TestMethod]
        public async Task TestRegisterGuild()
        {
            ulong guildId = 1525;

            Guild guild = await _guildRepository.RegisterGuild(guildId);
            Guild sameGuild = await _guildRepository.GetGuild(guildId);

            Assert.AreEqual(guild.GuildID, guildId);
            Assert.AreEqual(guild.GuildID, sameGuild.GuildID);
            Assert.AreEqual(guild.SystemChannelID, sameGuild.SystemChannelID);
            Assert.AreEqual(guild.Channels?.Count, guild.Channels?.Count);
        }
    }
}