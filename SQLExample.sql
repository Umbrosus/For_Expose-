Create procedure [dbo].[pr_CloseSepr_CloseSessionion]( @Sepr_CloseSessionionId int,@ErrorText nvarchar (1024)  out)
as
SET XACT_ABORT  on
declare  @trancnt int
set @ErrorText = ''
set @trancnt = @@TRANCOUNT
	begin try
	declare  @OrdersCount int
	if @trancnt = 0 BEGIN TRANSACTION 
	begin
	Select @OrdersCount=Count(*) from [dbo].[ReturnSepr_CloseSessionionHeader] as t1 
	inner join [dbo].[ReturnSepr_CloseSessionionDetail] as t2 on t1.ID = t2.Sepr_CloseSessionionID
	inner join [dbo].Orders as t3 on t2.OrderID = t3.Id
	Where t3.DcStatusId != 16 and t2.sepr_CloseSessionionID=@Sepr_CloseSessionionId
	if (@OrdersCount>0)
	begin
	Update [dbo].[ReturnSepr_CloseSessionionHeader] set EndDate = getDate() where id= @Sepr_CloseSessionionId

	declare @field1 int
	declare @ErrorCode int

declare cur CURSOR LOCAL for
    select OrderId from [dbo].[ReturnSepr_CloseSessionionDetail] where sepr_CloseSessionionID=@Sepr_CloseSessionionId and BoxCount = ScanedBoxCount

open cur

fetch next from cur into @field1 

while @@FETCH_STATUS = 0 BEGIN

    --execute your sproc on each row
    exec [dbo].[pr_SetOrderStatus] @field1,16,@ErrorCode,@ErrorText

    fetch next from cur into @field1
END

close cur
deallocate cur

	end
	else
	begin
		set  @ErrorText = ''
	end
	end
	if @trancnt = 0 COMMIT TRANSACTION
	end try
	begin catch
	   	if @tranCnt = 0 
	   	 begin
	   	    if @@trancount > 0 rollback tran
	   	 end
		set  @ErrorText = ERROR_MEpr_CloseSessionAGE()
	end catch