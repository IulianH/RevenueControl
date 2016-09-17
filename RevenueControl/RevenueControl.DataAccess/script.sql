USE [master]
GO
/****** Object:  Database [RevenueControl]    Script Date: 17.09.2016 18:02:00 ******/
CREATE DATABASE [RevenueControl]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RevenueControl.mdf', FILENAME = N'C:\Work\RevenueControl\RevenueControl\RevenueControl\RevenueControl.Web\App_Data\RevenueControl.mdf' , SIZE = 3264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'RevenueControl_log.ldf', FILENAME = N'C:\Work\RevenueControl\RevenueControl\RevenueControl\RevenueControl.Web\App_Data\RevenueControl_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [RevenueControl] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RevenueControl].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RevenueControl] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RevenueControl] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RevenueControl] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RevenueControl] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RevenueControl] SET ARITHABORT OFF 
GO
ALTER DATABASE [RevenueControl] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [RevenueControl] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RevenueControl] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RevenueControl] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RevenueControl] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RevenueControl] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RevenueControl] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RevenueControl] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RevenueControl] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RevenueControl] SET  ENABLE_BROKER 
GO
ALTER DATABASE [RevenueControl] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RevenueControl] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RevenueControl] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RevenueControl] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RevenueControl] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RevenueControl] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [RevenueControl] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RevenueControl] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [RevenueControl] SET  MULTI_USER 
GO
ALTER DATABASE [RevenueControl] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RevenueControl] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RevenueControl] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RevenueControl] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [RevenueControl] SET DELAYED_DURABILITY = DISABLED 
GO
USE [RevenueControl]
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 17.09.2016 18:02:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clients](
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DataSources]    Script Date: 17.09.2016 18:02:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataSources](
	[BankAccount] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Culture] [nvarchar](15) NOT NULL,
	[ClientName] [nvarchar](50) NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_DataSources] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_DataSources] UNIQUE CLUSTERED 
(
	[ClientName] ASC,
	[BankAccount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 17.09.2016 18:02:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[TransactionDetails] [nvarchar](max) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[TransactionType] [tinyint] NOT NULL,
	[OtherDetails] [nvarchar](max) NULL,
	[DataSourceId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Transactions] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Index [IX_TransactionDate]    Script Date: 17.09.2016 18:02:00 ******/
CREATE CLUSTERED INDEX [IX_TransactionDate] ON [dbo].[Transactions]
(
	[TransactionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DataSources]  WITH CHECK ADD  CONSTRAINT [FK_DataSources_Clients] FOREIGN KEY([ClientName])
REFERENCES [dbo].[Clients] ([Name])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DataSources] CHECK CONSTRAINT [FK_DataSources_Clients]
GO
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_DataSources] FOREIGN KEY([DataSourceId])
REFERENCES [dbo].[DataSources] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transactions_DataSources]
GO
USE [master]
GO
ALTER DATABASE [RevenueControl] SET  READ_WRITE 
GO
