using System;
using System.Linq;
using System.Text;
using NBitcoin.Altcoins.HashX11;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;

namespace NBitcoin.Altcoins
{
	// https://github.com/feirm/feirm/blob/master/src/chainparams.cpp
	public class Feirm : NetworkSetBase
	{
		public static Feirm Instance { get; } = new Feirm();

		public override string CryptoCode => "XFE";

		private Feirm()
		{
		}

		public class FeirmConsensusFactory : ConsensusFactory
		{
			private FeirmConsensusFactory()
			{
			}

			public static FeirmConsensusFactory Instance { get; } = new FeirmConsensusFactory();

			public override BlockHeader CreateBlockHeader()
			{
				return new FeirmBlockHeader();
			}
			public override Block CreateBlock()
			{
				return new FeirmBlock(new FeirmBlockHeader());
			}
		}


#pragma warning disable CS0618 // Type or member is obsolete
		public class FeirmBlockHeader : BlockHeader
		{
			// blob
			private static byte[] CalculateHash(byte[] data, int offset, int count)
			{
				return new HashX11.Quark().ComputeBytes(data.Skip(offset).Take(count).ToArray());
			}

			protected override HashStreamBase CreateHashStream()
			{
				return BufferedHashStream.CreateFrom(CalculateHash);
			}
		}

		public class FeirmBlock : Block
		{
#pragma warning disable CS0612 // Type or member is obsolete
			public FeirmBlock(FeirmBlockHeader h) : base(h)
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
			RegisterDefaultCookiePath("Feirm", new FolderName() { TestnetFolder = "testnet4" });
		}

		static uint256 GetPoWHash(BlockHeader header)
		{
			var headerBytes = header.ToBytes();
			var h = NBitcoin.Crypto.SCrypt.ComputeDerivedKey(headerBytes, headerBytes, 1024, 1, 1, null, 32);
			return new uint256(h);
		}

		protected override NetworkBuilder CreateMainnet()
		{
			var builder = new NetworkBuilder();
			builder.SetConsensus(new Consensus()
			{
				SubsidyHalvingInterval = 1050000,
				MajorityEnforceBlockUpgrade = 4050,
				MajorityRejectBlockOutdated = 5130,
				MajorityWindow = 5400,
				PowLimit = new Target(new uint256("0x00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
				PowTargetTimespan = TimeSpan.FromSeconds(60),
				PowTargetSpacing = TimeSpan.FromSeconds(120),
				PowAllowMinDifficultyBlocks = false,
				CoinbaseMaturity = 10,
				PowNoRetargeting = false,
				RuleChangeActivationThreshold = 10752,
				MinerConfirmationWindow = 13440,
				ConsensusFactory = FeirmConsensusFactory.Instance,
				SupportSegwit = false
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 12 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 57 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 55 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
			.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("xfe"))
			.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("xfe"))
			.SetMagic(0x9FA4FB4D)
			.SetPort(4918)
			.SetRPCPort(14000)
			.SetMaxP2PVersion(70917)
			.SetName("feirm-main")
			.AddAlias("feirm-mainnet")
			.AddDNSSeeds(new DNSSeedData[0])
			.AddSeeds(new NetworkAddress[0])
			.SetGenesis("e824b4355bb73c6072808d5bbb1624719c2d03d2c13429970a648efd2d4ceb08");
			return builder;
		}
		
		protected override NetworkBuilder CreateTestnet()
		{
			var builder = new NetworkBuilder();
			var res = builder.SetConsensus(new Consensus()
			{
				MajorityEnforceBlockUpgrade = 4320,
				MajorityRejectBlockOutdated = 5472,
				MajorityWindow = 5760,
				PowTargetTimespan = TimeSpan.FromSeconds(60),
				PowTargetSpacing = TimeSpan.FromSeconds(90),
				PowAllowMinDifficultyBlocks = false,
				CoinbaseMaturity = 15,
				ConsensusFactory = FeirmConsensusFactory.Instance,
				SupportSegwit = false
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 98 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 12 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 108 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x35, 0x87, 0xCF })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x35, 0x83, 0x94 })
			.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("txfe"))
			.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("txfe"))
			.SetMagic(0x5CBB1A33)
			.SetPort(34912)
			.SetRPCPort(14000)
			.SetMaxP2PVersion(70917)
			.SetName("feirm-test")
			.AddAlias("feirm-testnet")
			.AddDNSSeeds(new[]
			{
				new DNSSeedData("0",  "seed1.feirm.com"),
				new DNSSeedData("1",  "seed2.feirm.com"),
				new DNSSeedData("2",  "seed3.feirm.com"),
				new DNSSeedData("3",  "seed4.feirm.com"),
			})
			.AddSeeds(new NetworkAddress[0])
			.SetGenesis("e824b4355bb73c6072808d5bbb1624719c2d03d2c13429970a648efd2d4ceb08");
			return builder;
		}
		
		protected override NetworkBuilder CreateRegtest()
		{
			var builder = new NetworkBuilder();
			builder.SetConsensus(new Consensus()
			{
				SubsidyHalvingInterval = 150,
				MajorityEnforceBlockUpgrade = 750,
				MajorityRejectBlockOutdated = 950,
				MajorityWindow = 1000,
				PowTargetTimespan = TimeSpan.FromSeconds(24 * 60 * 60),
				PowTargetSpacing = TimeSpan.FromSeconds(2 * 60),
				PowAllowMinDifficultyBlocks = true,
				RuleChangeActivationThreshold = 108,
				MinerConfirmationWindow = 144,
				ConsensusFactory = FeirmConsensusFactory.Instance,
				SupportSegwit = false
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 98 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 12 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 108 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x35, 0x87, 0xCF })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x35, 0x83, 0x94 })
			.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("xfert"))
			.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("xfert"))
			.SetMagic(0xBC32EE20)
			.SetPort(81295)
			.SetRPCPort(14000)
			.SetMaxP2PVersion(70917)
			.SetName("feirm-reg")
			.AddAlias("feirm-regtest")
			.AddDNSSeeds(new DNSSeedData[0])
			.AddSeeds(new NetworkAddress[0])
			.SetGenesis("e824b4355bb73c6072808d5bbb1624719c2d03d2c13429970a648efd2d4ceb08");
			return builder;
		}
	}
}
