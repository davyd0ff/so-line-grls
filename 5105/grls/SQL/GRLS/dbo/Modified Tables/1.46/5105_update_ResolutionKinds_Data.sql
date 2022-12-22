USE [grls];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO


BEGIN TRAN;

BEGIN TRY
	BEGIN -- backup
		DECLARE @tableName VARCHAR(100)
		SET @tableName = '[GrlsHistory].[grls].ResolutionKinds_5105_'+REPLACE(REPLACE(REPLACE(CONVERT(varchar,GETDATE(), 120), '-', '_'), ' ', '_'), ':', '_');
		IF OBJECT_ID(@tableName) IS NULL BEGIN
			EXEC('SELECT * INTO '+@tableName+' FROM grls.dbo.ResolutionKinds') 
		END
	END;
	

	DECLARE @DEFECT_STATEMENT_CODE VARCHAR(50) = 'mp_defect_reg_statement';
	DECLARE @defect_statement_type_id INT = (select top(1) [idStatementType] from [dbo].[StatementTypeList] where [StatementTypeCode] = @DEFECT_STATEMENT_CODE);


	DECLARE @TASK_KIND_EXPERTISE_QUALITY_CODE NVARCHAR(100) = 'task_mr_em_q_riskexp_req_exp';
	DECLARE @TASK_KIND_EXPERTISE_QUALITY_FORCE_CODE NVARCHAR(100) = 'task_mr_em_q_riskexp_req_exp_force';
	DECLARE @TASK_KIND_ETHICS_CODE NVARCHAR(100) = 'task_mr_em_etexp_req_exp';
	DECLARE @TASK_KIND_ETHICS_FORCE_CODE NVARCHAR(100) = 'task_mr_em_etexp_req_exp_force';
	

	DECLARE @kindId INT = NULL,
			@kindCode NVARCHAR(100) = NULL,
			@postFix NVARCHAR(30) = NULL;
			
			
	BEGIN -- ЭКфПР
		SET @postFix = N'ЭКфПР';
		SET @kindCode = @TASK_KIND_EXPERTISE_QUALITY_CODE;

		SET @kindId = (select top(1) [ResolutionKindID] 
					   from [dbo].[ResolutionKinds] 
					   where [Code] = @kindCode 
					     and [StatementTypeId] = @defect_statement_type_id);
		
		IF @kindId IS NOT NULL
			UPDATE [dbo].[ResolutionKinds] 
			SET [PostFix] = @postFix 
			WHERE [ResolutionKindID] = @kindId;
	END;
	BEGIN -- ЭКфПР (force)
		SET @postFix = N'ЭКфПР';
		SET @kindCode = @TASK_KIND_EXPERTISE_QUALITY_FORCE_CODE;

		SET @kindId = (select top(1) [ResolutionKindID] 
					   from [dbo].[ResolutionKinds] 
					   where [Code] = @kindCode 
					     and [StatementTypeId] = @defect_statement_type_id);
		
		IF @kindId IS NOT NULL
			UPDATE [dbo].[ResolutionKinds] 
			SET [PostFix] = @postFix 
			WHERE [ResolutionKindID] = @kindId;
	END;
	BEGIN -- ЭТ
		SET @postFix = N'ЭТ';
		SET @kindCode = @TASK_KIND_ETHICS_CODE;

		SET @kindId = (select top(1) [ResolutionKindID] 
					   from [dbo].[ResolutionKinds] 
					   where [Code] = @kindCode 
					     and [StatementTypeId] = @defect_statement_type_id);
		
		IF @kindId IS NOT NULL
			UPDATE [dbo].[ResolutionKinds] 
			SET [PostFix] = @postFix 
			WHERE [ResolutionKindID] = @kindId;
	END;
	BEGIN -- ЭТ (force)
		SET @postFix = N'ЭТ';
		SET @kindCode = @TASK_KIND_ETHICS_FORCE_CODE;

		SET @kindId = (select top(1) [ResolutionKindID] 
					   from [dbo].[ResolutionKinds] 
					   where [Code] = @kindCode 
					     and [StatementTypeId] = @defect_statement_type_id);
		
		IF @kindId IS NOT NULL
			UPDATE [dbo].[ResolutionKinds] 
			SET [PostFix] = @postFix 
			WHERE [ResolutionKindID] = @kindId;
	END;


	DECLARE @resolution_pair_id INT = NULL,
			@task_pair_id INT = NULL;

	BEGIN -- чиним ResolutionKindPairId
		BEGIN -- ЭКфПР
			DECLARE @RESOLUTION_KIND_EXPERTISE_QUALITY_CODE NVARCHAR(100) = 'resolution_mr_em_q_riskexp_req';
			
			SET @resolution_pair_id = (select [ResolutionKindID] 
			                           from [dbo].[ResolutionKinds] 
									   where [Code] = @RESOLUTION_KIND_EXPERTISE_QUALITY_CODE
										 and [StatementTypeId] = @defect_statement_type_id);
			SET @task_pair_id = (select [ResolutionKindID] 
			                     from [dbo].[ResolutionKinds] 
								 where [Code] = @TASK_KIND_EXPERTISE_QUALITY_CODE
								   and [StatementTypeId] = @defect_statement_type_id);

			IF @resolution_pair_id IS NOT NULL AND @task_pair_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionKinds]
				SET [ResolutionKindPairId] = @resolution_pair_id
				WHERE [ResolutionKindID] = @task_pair_id;
			END
		END
		BEGIN -- ЭКфПР (force)
			DECLARE @RESOLUTION_KIND_EXPERTISE_QUALITY_FORCE_CODE NVARCHAR(100) = 'resolution_mr_em_q_riskexp_req_force';
			
			SET @resolution_pair_id = (select [ResolutionKindID] 
			                           from [dbo].[ResolutionKinds] 
									   where [Code] = @RESOLUTION_KIND_EXPERTISE_QUALITY_FORCE_CODE
										 and [StatementTypeId] = @defect_statement_type_id);
			SET @task_pair_id = (select [ResolutionKindID] 
			                     from [dbo].[ResolutionKinds] 
								 where [Code] = @TASK_KIND_EXPERTISE_QUALITY_FORCE_CODE
								   and [StatementTypeId] = @defect_statement_type_id);

			IF @resolution_pair_id IS NOT NULL AND @task_pair_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionKinds]
				SET [ResolutionKindPairId] = @resolution_pair_id
				WHERE [ResolutionKindID] = @task_pair_id;
			END
		END
		BEGIN -- ЭТ
			DECLARE @RESOLUTION_KIND_ETHICS_CODE NVARCHAR(100) = 'resolution_mr_em_etexp_req';
			
			SET @resolution_pair_id = (select [ResolutionKindID] 
			                           from [dbo].[ResolutionKinds] 
									   where [Code] = @RESOLUTION_KIND_ETHICS_CODE
										 and [StatementTypeId] = @defect_statement_type_id);
			SET @task_pair_id = (select [ResolutionKindID] 
			                     from [dbo].[ResolutionKinds] 
								 where [Code] = @TASK_KIND_ETHICS_CODE
								   and [StatementTypeId] = @defect_statement_type_id);

			IF @resolution_pair_id IS NOT NULL AND @task_pair_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionKinds]
				SET [ResolutionKindPairId] = @resolution_pair_id
				WHERE [ResolutionKindID] = @task_pair_id;
			END
		END

		BEGIN -- ЭТ (force)
			DECLARE @RESOLUTION_KIND_ETHICS_FORCE_CODE NVARCHAR(100) = 'resolution_mr_em_etexp_req_force';
			
			SET @resolution_pair_id = (select [ResolutionKindID] 
			                           from [dbo].[ResolutionKinds] 
									   where [Code] = @RESOLUTION_KIND_ETHICS_FORCE_CODE
										 and [StatementTypeId] = @defect_statement_type_id);
			SET @task_pair_id = (select [ResolutionKindID] 
			                     from [dbo].[ResolutionKinds] 
								 where [Code] = @TASK_KIND_ETHICS_FORCE_CODE
								   and [StatementTypeId] = @defect_statement_type_id);

			IF @resolution_pair_id IS NOT NULL AND @task_pair_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionKinds]
				SET [ResolutionKindPairId] = @resolution_pair_id
				WHERE [ResolutionKindID] = @task_pair_id;
			END
		END
	END


	COMMIT TRAN;
END TRY
BEGIN CATCH

	ROLLBACK TRAN;
END CATCH
