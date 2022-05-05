
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE procedure [dbo].[ImportExtension]
  @ObjectId as int,
  @Body as xml
as
begin
  set nocount on;
  BEGIN TRY
    IF @@TRANCOUNT = 0 RAISERROR('No external transaction', 16, 1)
    declare @LongKey  nvarchar(max)
    declare @ShortKey nvarchar(max)
    declare @ExtensionId int
    declare @TypeId int
    declare @Type nvarchar(max)
    declare @IsCritical bit
    select top 1
       @IsCritical =
        case(lower([a].value(N'@IsCritical[1]' ,N'nvarchar(max)')))
          when 'true' then 1
          else 0
        end
      ,@Type = [a].value(N'@Identifier[1]',N'nvarchar(max)')
    from @Body.nodes(N'Extension') [a]([a])
    execute [dbo].[ImportObjectIdentifier] @Value=@Type,@Identifier=@TypeId output
    insert into [Extension] ([ObjectId],[Type],[IsCritical]) values (@ObjectId,@TypeId,@IsCritical)
    set @ExtensionId=@@identity
    if @Type='2.5.29.14'
    begin
      select top 1
         @LongKey = [a].value(N'@Key[1]',N'nvarchar(max)')
      from @Body.nodes(N'//CertificateSubjectKeyIdentifier') [a]([a])
      insert into [dbo].[SubjectKeyIdentifier]
        ([ExtensionId],[LongKey],[ShortKey]) values
        (@ExtensionId, @LongKey, [dbo].[GetShortKey](@LongKey))
    end
    else if @Type='2.5.29.35'
    begin
      declare @CertificateIssuerId int
      declare @CertificateIssuer xml
      declare @SerialNumber nvarchar(max)
      select top 1
          @LongKey = [a].value(N'@Key[1]',N'nvarchar(max)')
         ,@SerialNumber= [a].value(N'@SerialNumber[1]',N'nvarchar(max)')
         ,@CertificateIssuer = [a].query('CertificateIssuer/RelativeDistinguishedName')
      from @Body.nodes(N'//CertificateAuthorityKeyIdentifier') [a]([a])
      execute [dbo].[ImportRelativeDistinguishedNameSequence] @Body=@CertificateIssuer ,@Identifier=@CertificateIssuerId output
      insert into [dbo].[AuthorityKeyIdentifier]
        ([ExtensionId],[LongKey],[ShortKey],[SerialNumber],[Issuer]) values
        (@ExtensionId, @LongKey, [dbo].[GetShortKey](@LongKey),@SerialNumber,@CertificateIssuerId)
    end
  END   TRY
  BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(MAX)
    DECLARE @ErrorSeverity NVARCHAR(MAX)
    DECLARE @ErrorState NVARCHAR(MAX)
    DECLARE @ErrorNumber NVARCHAR(MAX)
    DECLARE @ErrorLine NVARCHAR(MAX)
    DECLARE @ErrorProcedure NVARCHAR(MAX)

    SELECT
      @ErrorMessage   = ERROR_MESSAGE()
     ,@ErrorSeverity  = CAST(ERROR_SEVERITY() AS NVARCHAR(MAX))
     ,@ErrorState     = CAST(ERROR_STATE() AS NVARCHAR(MAX))
     ,@ErrorNumber    = CAST(ERROR_NUMBER() AS NVARCHAR(MAX))
     ,@ErrorLine      = CAST(ERROR_LINE() AS NVARCHAR(MAX))
     ,@ErrorProcedure = CAST(ERROR_PROCEDURE() AS NVARCHAR(MAX))

    SET @ErrorMessage = @ErrorMessage +
      N':{Number='+@ErrorNumber+
      '},{State='+ @ErrorState +
      '},{Severity='+ @ErrorSeverity +
      '},{Line='+ @ErrorLine +
      '},{Procedure='+ @ErrorProcedure +'}'
    IF @@TRANCOUNT > 0 ROLLBACK TRAN
    RAISERROR(@ErrorMessage, 11, 1)
  END   CATCH
END