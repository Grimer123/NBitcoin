using System;
using System.Linq;
using System.Text;
using NBitcoin.Altcoins.HashX11;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;

namespace NBitcoin.Altcoins
{
	// https://github.com/MONKEYPROJECT/MonkeyV2/blob/v2.3.0/src/chainparams.cpp
	public class Monkey : NetworkSetBase
	{
		public static Monkey Instance { get; } = new Monkey();

		public override string CryptoCode => "MONK";

		private Monkey()
		{
		}

		public class MonkeyConsensusFactory : ConsensusFactory
		{
			private MonkeyConsensusFactory()
			{
			}

			public static MonkeyConsensusFactory Instance { get; } = new MonkeyConsensusFactory();

			public override BlockHeader CreateBlockHeader()
			{
				return new MonkeyBlockHeader();
			}
			public override Block CreateBlock()
			{
				return new MonkeyBlock(new MonkeyBlockHeader());
			}
		}


#pragma warning disable CS0618 // Type or member is obsolete
		public class MonkeyBlockHeader : BlockHeader
		{
			// blob
			private static byte[] CalculateHash(byte[] data, int offset, int count)
			{
				return new Quark().ComputeBytes(data.Skip(offset).Take(count).ToArray());
			}

			protected override HashStreamBase CreateHashStream()
			{
				return BufferedHashStream.CreateFrom(CalculateHash);
			}
		}

		public class MonkeyBlock : Block
		{
#pragma warning disable CS0612 // Type or member is obsolete
			public MonkeyBlock(MonkeyBlockHeader h) : base(h)
#pragma warning restore CS0612 // Type or member is obsolete
			{

			}
			public override ConsensusFactory GetConsensusFactory()
			{
				return Instance.Mainnet.Consensus.ConsensusFactory;
			}
		}
#pragma warning restore CS0618 // Type or member is obsolete

		protected override void PostInit()
		{
			RegisterDefaultCookiePath("Monkey", new FolderName() { TestnetFolder = "testnetmonk" });
		}

		static uint256 GetPoWHash(BlockHeader header)
		{
			var headerBytes = header.ToBytes();
			var h = NBitcoin.Crypto.SCrypt.ComputeDerivedKey(headerBytes, headerBytes, 1024, 1, 1, null, 64);
			return new uint256(h);
		}

		protected override NetworkBuilder CreateMainnet()
		{
			var builder = new NetworkBuilder();
			builder.SetConsensus(new Consensus()
			{
				MajorityEnforceBlockUpgrade = 750,
				MajorityRejectBlockOutdated = 950,
				MajorityWindow = 1000,
				PowAllowMinDifficultyBlocks = false,
				CoinbaseMaturity = 10,
				PowNoRetargeting = false,
				RuleChangeActivationThreshold = 1916,
				MinerConfirmationWindow = 2016,
				ConsensusFactory = MonkeyConsensusFactory.Instance,
				SupportSegwit = false
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 51 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 28 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 55 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
			.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("monk"))
			.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("monk"))
			.SetMagic(0x96492351)
			.SetPort(37233)
			.SetRPCPort(21000)
			.SetMaxP2PVersion(70913)
			.SetName("monkey-main")
			.AddAlias("monkey-mainnet")
			.AddDNSSeeds(new[]
			{
				new DNSSeedData("1", "seed1.monk3y.xyz"),
				new DNSSeedData("2", "seed2.monk3y.xyz"),
				new DNSSeedData("3", "seed3.monk3y.xyz"),
			})
			.AddSeeds(new NetworkAddress[0])
			.SetGenesis("010000000000000000000000000000000000000000000000000000000000000000000000887c5c20f3075215e164877a6de732695a13c0f8ec0fcf6296fa942487f96efadae5494dffff7f20000000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d01044531382d30312d3134202d20416e74692d667261636b696e672063616d706169676e65727320636861696e207468656d73656c76657320746f20706574726f6c2070756d7073ffffffff0100105e5f00000000434104becedf6ebadd4596964d890f677f8d2e74fdcc313c6416434384a66d6d8758d1c92de272dc6713e4a81d98841dfdfdc95e204ba915447d2fe9313435c78af3e8ac00000000");
			return builder;
		}
		
		protected override NetworkBuilder CreateTestnet()
		{
			var builder = new NetworkBuilder();
			var res = builder.SetConsensus(new Consensus()
			{
				MajorityEnforceBlockUpgrade = 51,
				MajorityRejectBlockOutdated = 75,
				MajorityWindow = 100,
				PowAllowMinDifficultyBlocks = true,
				CoinbaseMaturity = 15,
				PowNoRetargeting = false,
				RuleChangeActivationThreshold = 1916,
				MinerConfirmationWindow = 2016,
				ConsensusFactory = MonkeyConsensusFactory.Instance,
				SupportSegwit = false
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 127 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 96 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 63 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x35, 0x87, 0xCF })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x35, 0x83, 0x94 })
			.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("tmonk"))
			.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("tmonk"))
			.SetMagic(0x8C0DBEE1)
			.SetPort(47233)
			.SetRPCPort(21001)
			.SetMaxP2PVersion(70913)
			.SetName("monkey-test")
			.AddAlias("monkey-testnet")
			.AddSeeds(new NetworkAddress[0])
			.SetGenesis("010000000000000000000000000000000000000000000000000000000000000000000000887c5c20f3075215e164877a6de732695a13c0f8ec0fcf6296fa942487f96efadae5494dffff7f20000000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d01044531382d30312d3134202d20416e74692d667261636b696e672063616d706169676e65727320636861696e207468656d73656c76657320746f20706574726f6c2070756d7073ffffffff0100105e5f00000000434104becedf6ebadd4596964d890f677f8d2e74fdcc313c6416434384a66d6d8758d1c92de272dc6713e4a81d98841dfdfdc95e204ba915447d2fe9313435c78af3e8ac00000000");
			return builder;
		}
		
		protected override NetworkBuilder CreateRegtest()
		{
			var builder = new NetworkBuilder();
			builder.SetConsensus(new Consensus()
			{
				MajorityEnforceBlockUpgrade = 750,
				MajorityRejectBlockOutdated = 950,
				MajorityWindow = 1000,
				PowAllowMinDifficultyBlocks = true,
				CoinbaseMaturity = 10,
				PowNoRetargeting = true,
				RuleChangeActivationThreshold = 1916,
				MinerConfirmationWindow = 2016,
				ConsensusFactory = MonkeyConsensusFactory.Instance,
				SupportSegwit = false
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 127 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 127 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 63 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
			.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("monkrt"))
			.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("monkrt"))
			.SetMagic(0xECC0988F)
			.SetPort(57233)
			.SetRPCPort(21002)
			.SetMaxP2PVersion(70913)
			.SetName("monkey-reg")
			.AddAlias("monkey-regtest")
			.AddDNSSeeds(new DNSSeedData[0])
			.AddSeeds(new NetworkAddress[0])
			.SetGenesis("010000000000000000000000000000000000000000000000000000000000000000000000887c5c20f3075215e164877a6de732695a13c0f8ec0fcf6296fa942487f96efadae5494dffff7f20000000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d01044531382d30312d3134202d20416e74692d667261636b696e672063616d706169676e65727320636861696e207468656d73656c76657320746f20706574726f6c2070756d7073ffffffff0100105e5f00000000434104becedf6ebadd4596964d890f677f8d2e74fdcc313c6416434384a66d6d8758d1c92de272dc6713e4a81d98841dfdfdc95e204ba915447d2fe9313435c78af3e8ac00000000");
			return builder;
		}
	}
}
