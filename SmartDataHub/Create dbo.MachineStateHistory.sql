USE [SmartDataHubContext-06372eea-a0ea-43d8-8c67-d0a88d838035]
GO

/****** Objekt: Table [dbo].[CycleMachineConfiguration] Skriptdatum: 27.12.2017 00:02:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MachineStateHistory] (
    [MachineStateHistoryId] INT            IDENTITY (1, 1) NOT NULL,
    [MachineId]               INT            NOT NULL,
    [MachineState]                INT NOT NULL,
    [StartDateTime]         DATETIME            NOT NULL,
    [EndDateTime]         DATETIME            NOT NULL,
    [Duration]                PERIOD            NOT NULL,
	[DailyCycleCounter]                INT NOT NULL,
	[CyclesInThisPeriod]                INT NOT NULL
);


--GO
--CREATE NONCLUSTERED INDEX [IX_CycleMachineConfiguration_SmartAgentId]
--    ON [dbo].[CycleMachineConfiguration]([SmartAgentId] ASC);


GO
ALTER TABLE [dbo].[MachineStateHistory]
    ADD CONSTRAINT [PK_MachineStateHistory] PRIMARY KEY CLUSTERED ([MachineStateHistoryId] ASC);


GO
ALTER TABLE [dbo].[MachineStateHistory]
    ADD CONSTRAINT [FK_MachineStateHistory_CycleMachineConfiguration_CycleMachineConfigurationId] FOREIGN KEY ([MachineId]) REFERENCES [dbo].[CycleMachineConfiguration] ([CycleMachineConfigurationId]) ON DELETE CASCADE;


