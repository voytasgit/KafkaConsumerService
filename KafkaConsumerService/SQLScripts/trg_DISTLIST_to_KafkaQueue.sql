USE [adito_dev]
GO

/****** Object:  Trigger [dbo].[trg_DISTLIST_to_KafkaQueue]    Script Date: 08.05.2025 17:51:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[trg_DISTLIST_to_KafkaQueue]
ON [dbo].[DISTLIST]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    DECLARE @ID char(36)= newid();
    DECLARE @Action VARCHAR(50);

    -- Überprüfen, ob es eine INSERT-Operation gab
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        SET @Action = 'INSERT';
    END

    -- Überprüfen, ob es eine DELETE-Operation gab
    IF EXISTS (SELECT 1 FROM deleted)
    BEGIN
        SET @Action = 'DELETE';
    END

    -- Überprüfen, ob es eine UPDATE-Operation gab
    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
    BEGIN
        SET @Action = 'UPDATE';
    END

    -- INSERT-Operation (neue Zeilen)
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[AP_KAFKA_QUEUE] (QUEUE_ID, TABLE_NAME, ACTION_NAME, KEY_VALUE, DATA, ProcessingStatus)
        SELECT @ID, 'DISTLIST', @Action, i.DISTLISTID, 
               CONCAT('{"DISTLISTID": "', i.DISTLISTID, '", "DESCRIPTION": "', i.DESCRIPTION, '", ', CASE WHEN i.OWNER_ID IS NULL THEN '"OWNER_ID": null'  ELSE '"OWNER_ID": ' + CAST(i.OWNER_ID AS varchar) END, ', "IS_DYNAMIC": "', i.IS_DYNAMIC, '", "NAME": "', i.NAME, '", "SELECTION_ID": "', i.SELECTION_ID, '", "DATE_NEW": "', FORMAT(i.DATE_NEW, 'yyyy-MM-ddTHH:mm:ss'), '", "DATE_EDIT": "', FORMAT(i.DATE_EDIT, 'yyyy-MM-ddTHH:mm:ss'), '", "USER_NEW": "', i.USER_NEW, '", "USER_EDIT": "', i.USER_EDIT, '", "AKTIV": "', i.AKTIV, '"}'),
               'Pending'
        FROM inserted i;
    END

    -- DELETE-Operation (gelöschte Zeilen)
    IF @Action = 'DELETE'
    BEGIN
        INSERT INTO [dbo].[AP_KAFKA_QUEUE] (QUEUE_ID, TABLE_NAME, ACTION_NAME, KEY_VALUE, DATA, ProcessingStatus)
        SELECT @ID,'DISTLIST', @Action, d.DISTLISTID, 
               CONCAT('{"DISTLISTID": "', d.DISTLISTID, '", "DESCRIPTION": "', d.DESCRIPTION, '", ', CASE WHEN d.OWNER_ID IS NULL THEN '"OWNER_ID": null'  ELSE '"OWNER_ID": ' + CAST(d.OWNER_ID AS varchar) END, '", "IS_DYNAMIC": "', d.IS_DYNAMIC, '", "NAME": "', d.NAME, '", "SELECTION_ID": "', d.SELECTION_ID, '", "DATE_NEW": "', FORMAT(d.DATE_NEW, 'yyyy-MM-ddTHH:mm:ss'), '", "DATE_EDIT": "', FORMAT(d.DATE_EDIT, 'yyyy-MM-ddTHH:mm:ss'), '", "USER_NEW": "', d.USER_NEW, '", "USER_EDIT": "', d.USER_EDIT, '", "AKTIV": "', d.AKTIV, '"}'),
               'Pending'
        FROM deleted d;
    END

    -- UPDATE-Operation (geänderte Zeilen)
    IF @Action = 'UPDATE'
    BEGIN
        INSERT INTO [dbo].[AP_KAFKA_QUEUE] (QUEUE_ID, TABLE_NAME, ACTION_NAME, KEY_VALUE, DATA, ProcessingStatus)
        SELECT @ID, 'DISTLIST', @Action, i.DISTLISTID, 
               CONCAT('{"DISTLISTID": "', i.DISTLISTID, '", "DESCRIPTION": "', i.DESCRIPTION, '", ', CASE WHEN i.OWNER_ID IS NULL THEN '"OWNER_ID": null'  ELSE '"OWNER_ID": ' + CAST(i.OWNER_ID AS varchar) END, '", "IS_DYNAMIC": "', i.IS_DYNAMIC, '", "NAME": "', i.NAME, '", "SELECTION_ID": "', i.SELECTION_ID, '", "DATE_NEW": "', FORMAT(i.DATE_NEW, 'yyyy-MM-ddTHH:mm:ss'), '", "DATE_EDIT": "', FORMAT(i.DATE_EDIT, 'yyyy-MM-ddTHH:mm:ss'), '", "USER_NEW": "', i.USER_NEW, '", "USER_EDIT": "', i.USER_EDIT, '", "AKTIV": "', i.AKTIV, 
                      '", "OLD_DESCRIPTION": "', d.DESCRIPTION, '", "OLD_OWNER_ID": "', d.OWNER_ID, '", "OLD_IS_DYNAMIC": "', d.IS_DYNAMIC, '", "OLD_NAME": "', d.NAME, '", "OLD_SELECTION_ID": "', d.SELECTION_ID, '", "OLD_DATE_NEW": "', d.DATE_NEW, '", "OLD_DATE_EDIT": "', d.DATE_EDIT, '", "OLD_USER_NEW": "', d.USER_NEW, '", "OLD_USER_EDIT": "', d.USER_EDIT, '", "OLD_AKTIV": "', d.AKTIV, '"}'),
               'Pending'
        FROM inserted i
        JOIN deleted d ON i.DISTLISTID = d.DISTLISTID;
    END
END;

GO

ALTER TABLE [dbo].[DISTLIST] ENABLE TRIGGER [trg_DISTLIST_to_KafkaQueue]
GO


