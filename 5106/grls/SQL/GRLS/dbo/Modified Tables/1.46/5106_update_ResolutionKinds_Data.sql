USE [grls];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO


DECLARE @DEFECT_STATEMENT_CODE VARCHAR(50) = 'mp_defect_reg_statement';
DECLARE @defect_statement_type_id INT = (select top(1) [idStatementType] from [dbo].[StatementTypeList] where [StatementTypeCode] = @DEFECT_STATEMENT_CODE);


DECLARE @RESOLUTION_KIND_EXPERTISE_QUALITY_CODE NVARCHAR(100) = 'resolution_mr_em_q_riskexp_req';
DECLARE @RESOLUTION_KIND_EXPERTISE_QUALITY_FORCE_CODE NVARCHAR(100) = 'resolution_mr_em_q_riskexp_req_force';
DECLARE @RESOLUTION_KIND_ETHICS_CODE NVARCHAR(100) = 'resolution_mr_em_etexp_req';
DECLARE @RESOLUTION_KIND_ETHICS_FORCE_CODE NVARCHAR(100) = 'resolution_mr_em_etexp_req_force';
DECLARE @RESOLUTION_KIND_REG_ALLOW_CODE NVARCHAR(100) = 'resolution_mr_em_allow_req';
DECLARE @RESOLUTION_KIND_REG_FORCE_ALLOW_CODE NVARCHAR(100) = 'resolution_mr_em_allow_req_force';
DECLARE @RESOLUTION_KIND_EMERGENCY_REG_ALLOW_CODE NVARCHAR(100) = 'resolution_mr_emergency_em_allow_req';
DECLARE @RESOLUTION_KIND_REG_REJECT_CODE NVARCHAR(100) = 'resolution_mr_emergency_reject_fail_req';
DECLARE @RESOLUTION_KIND_REG_REJECT_FORCE_CODE NVARCHAR(100) = 'resolution_mr_emergency_reject_fail_force__req';



BEGIN -- ÔÓ‚ÂÍ‡ Ì‡ Ó·ÌÓ‚ÎÂÌËÂ
	DECLARE @resolution_kind_code NVARCHAR(100) = NULL,
			@resolution_kind_id INT = NULL,
			@resolution_need_changing BIT = NULL,
			@resolution_postfix NVARCHAR(30) = NULL;

	BEGIN -- › Ùœ–
		SET @resolution_kind_code = NULL;
		SET @resolution_kind_id = NULL;
		SET @resolution_need_changing = 1;
		SET @resolution_postfix = NULL;

		SET @resolution_kind_code = @RESOLUTION_KIND_EXPERTISE_QUALITY_CODE;
		DECLARE @resolution_kind_expertise_quality__postfix NVARCHAR(30) = '› Ùœ–';
		
		SET @resolution_postfix = @resolution_kind_expertise_quality__postfix;
		SET @resolution_kind_id = (select top(1) [ResolutionKindID]
								   from [dbo].[ResolutionKinds]
								   where [Code] = @resolution_kind_code
								     and [StatementTypeId] = @defect_statement_type_id);

		SET @resolution_need_changing = (isNull((select 0 
										  	     from [dbo].[ResolutionKinds]
											     where [ResolutionKindID] = @resolution_kind_id
											       and [PostFix] = @resolution_postfix
									     ), 1));
		
		DECLARE @resolution_kind_expertise_quality_id INT = @resolution_kind_id;
		DECLARE @resolution_kind_expertise_quality_need_changing BIT = @resolution_need_changing;
	END;
	BEGIN -- › Ùœ– (force)
		SET @resolution_kind_code = NULL;
		SET @resolution_kind_id = NULL;
		SET @resolution_need_changing = 1;
		SET @resolution_postfix = NULL;

		SET @resolution_kind_code = @RESOLUTION_KIND_EXPERTISE_QUALITY_FORCE_CODE;
		DECLARE @resolution_kind_expertise_quality_force__postfix NVARCHAR(30) = '› Ùœ–';

		SET @resolution_postfix = @resolution_kind_expertise_quality_force__postfix;
		SET @resolution_kind_id = (select top(1) [ResolutionKindID]
								   from [dbo].[ResolutionKinds]
								   where [Code] = @resolution_kind_code
								     and [StatementTypeId] = @defect_statement_type_id);

		SET @resolution_need_changing = (isNull((select 0 
										  	     from [dbo].[ResolutionKinds]
											     where [ResolutionKindID] = @resolution_kind_id
											       and [PostFix] = @resolution_postfix
									     ), 1));

		DECLARE @resolution_kind_expertise_quality_force_id INT = @resolution_kind_id;
		DECLARE @resolution_kind_expertise_quality_force_need_changing BIT = @resolution_need_changing;
	END;
	BEGIN -- ›“
		SET @resolution_kind_code = NULL;
		SET @resolution_kind_id = NULL;
		SET @resolution_need_changing = 1;
		SET @resolution_postfix = NULL;

		SET @resolution_kind_code = @RESOLUTION_KIND_ETHICS_CODE;
		DECLARE @resolution_kind_ethics__postfix NVARCHAR(30) = '›“';

		SET @resolution_postfix = @resolution_kind_ethics__postfix;
		SET @resolution_kind_id = (select top(1) [ResolutionKindID]
								   from [dbo].[ResolutionKinds]
								   where [Code] = @resolution_kind_code
								     and [StatementTypeId] = @defect_statement_type_id);

		SET @resolution_need_changing = (isNull((select 0 
										  	     from [dbo].[ResolutionKinds]
											     where [ResolutionKindID] = @resolution_kind_id
											       and [PostFix] = @resolution_postfix
									     ), 1));

		DECLARE @resolution_kind_ethics_id INT = @resolution_kind_id;
		DECLARE @resolution_kind_ethics_need_changing BIT = @resolution_need_changing;
	END;
	BEGIN -- ›“ (force)
		SET @resolution_kind_code = NULL;
		SET @resolution_kind_id = NULL;
		SET @resolution_need_changing = 1;
		SET @resolution_postfix = NULL;

		SET @resolution_kind_code = @RESOLUTION_KIND_ETHICS_FORCE_CODE;
		DECLARE @resolution_kind_ethics_force_quality__postfix NVARCHAR(30) = '›“';

		SET @resolution_postfix = @resolution_kind_ethics_force_quality__postfix;
		SET @resolution_kind_id = (select top(1) [ResolutionKindID]
								   from [dbo].[ResolutionKinds]
								   where [Code] = @resolution_kind_code
								     and [StatementTypeId] = @defect_statement_type_id);

		SET @resolution_need_changing = (isNull((select 0 
										  	     from [dbo].[ResolutionKinds]
											     where [ResolutionKindID] = @resolution_kind_id
											       and [PostFix] = @resolution_postfix
									     ), 1));

		DECLARE @resolution_kind_ethics_force_quality_id INT = @resolution_kind_id;
		DECLARE @resolution_kind_ethics_force_quality_need_changing BIT = @resolution_need_changing;
	END;
	BEGIN -- –Àœ
		SET @resolution_kind_code = NULL;
		SET @resolution_kind_id = NULL;
		SET @resolution_need_changing = 1;
		SET @resolution_postfix = NULL;

		SET @resolution_kind_code = @RESOLUTION_KIND_REG_ALLOW_CODE;
		DECLARE @resolution_kind_reg_allow__postfix NVARCHAR(30) = '–Àœ';

		SET @resolution_postfix = @resolution_kind_reg_allow__postfix;
		SET @resolution_kind_id = (select top(1) [ResolutionKindID]
								   from [dbo].[ResolutionKinds]
								   where [Code] = @resolution_kind_code
								     and [StatementTypeId] = @defect_statement_type_id);

		SET @resolution_need_changing = (isNull((select 0 
										  	     from [dbo].[ResolutionKinds]
											     where [ResolutionKindID] = @resolution_kind_id
											       and [PostFix] = @resolution_postfix
									     ), 1));

		DECLARE @resolution_kind_reg_allow_id INT = @resolution_kind_id;
		DECLARE @resolution_kind_reg_allow_need_changing BIT = @resolution_need_changing;
	END;
	BEGIN -- –Àœ (force)
		SET @resolution_kind_code = NULL;
		SET @resolution_kind_id = NULL;
		SET @resolution_need_changing = 1;
		SET @resolution_postfix = NULL;

		SET @resolution_kind_code = @RESOLUTION_KIND_REG_FORCE_ALLOW_CODE;
		DECLARE @resolution_kind_reg_allow_force__postfix NVARCHAR(30) = '–Àœ';

		SET @resolution_postfix = @resolution_kind_reg_allow_force__postfix;
		SET @resolution_kind_id = (select top(1) [ResolutionKindID]
								   from [dbo].[ResolutionKinds]
								   where [Code] = @resolution_kind_code
								     and [StatementTypeId] = @defect_statement_type_id);

		SET @resolution_need_changing = (isNull((select 0 
										  	     from [dbo].[ResolutionKinds]
											     where [ResolutionKindID] = @resolution_kind_id
											       and [PostFix] = @resolution_postfix
									     ), 1));

		DECLARE @resolution_kind_reg_allow_force_id INT = @resolution_kind_id;
		DECLARE @resolution_kind_reg_allow_force_need_changing BIT = @resolution_need_changing;
	END;
	BEGIN -- –Àœ (emergency)
		SET @resolution_kind_code = NULL;
		SET @resolution_kind_id = NULL;
		SET @resolution_need_changing = 1;
		SET @resolution_postfix = NULL;

		SET @resolution_kind_code = @RESOLUTION_KIND_EMERGENCY_REG_ALLOW_CODE;
		DECLARE @resolution_kind_emergency_reg_allow__postfix NVARCHAR(30) = '–Àœ';

		SET @resolution_postfix = @resolution_kind_emergency_reg_allow__postfix;
		SET @resolution_kind_id = (select top(1) [ResolutionKindID]
								   from [dbo].[ResolutionKinds]
								   where [Code] = @resolution_kind_code
								     and [StatementTypeId] = @defect_statement_type_id);

		SET @resolution_need_changing = (isNull((select 0 
										  	     from [dbo].[ResolutionKinds]
											     where [ResolutionKindID] = @resolution_kind_id
											       and [PostFix] = @resolution_postfix
									     ), 1));

		DECLARE @resolution_kind_emergency_reg_allow_id INT = @resolution_kind_id;
		DECLARE @resolution_kind_emergency_reg_allow_need_changing BIT = @resolution_need_changing;
	END;
	BEGIN -- –ÀœŒ 
		SET @resolution_kind_code = NULL;
		SET @resolution_kind_id = NULL;
		SET @resolution_need_changing = 1;
		SET @resolution_postfix = NULL;

		SET @resolution_kind_code = @RESOLUTION_KIND_REG_REJECT_CODE;
		DECLARE @resolution_kind_reg_reject__postfix NVARCHAR(30) = '–ÀœŒ';

		SET @resolution_postfix = @resolution_kind_reg_reject__postfix;
		SET @resolution_kind_id = (select top(1) [ResolutionKindID]
								   from [dbo].[ResolutionKinds]
								   where [Code] = @resolution_kind_code
								     and [StatementTypeId] = @defect_statement_type_id);

		SET @resolution_need_changing = (isNull((select 0 
										  	     from [dbo].[ResolutionKinds]
											     where [ResolutionKindID] = @resolution_kind_id
											       and [PostFix] = @resolution_postfix
									     ), 1));

		DECLARE @resolution_kind_reg_reject_id INT = @resolution_kind_id;
		DECLARE @resolution_kind_reg_reject_need_changing BIT = @resolution_need_changing;
	END;
	BEGIN -- –ÀœŒ (force)
		SET @resolution_kind_code = NULL;
		SET @resolution_kind_id = NULL;
		SET @resolution_need_changing = 1;
		SET @resolution_postfix = NULL;

		SET @resolution_kind_code = @RESOLUTION_KIND_REG_REJECT_FORCE_CODE;
		DECLARE @resolution_kind_reg_reject_force__postfix NVARCHAR(30) = '–ÀœŒ';

		SET @resolution_postfix = @resolution_kind_reg_reject_force__postfix;
		SET @resolution_kind_id = (select top(1) [ResolutionKindID]
								   from [dbo].[ResolutionKinds]
								   where [Code] = @resolution_kind_code
								     and [StatementTypeId] = @defect_statement_type_id);

		SET @resolution_need_changing = (isNull((select 0 
										  	     from [dbo].[ResolutionKinds]
											     where [ResolutionKindID] = @resolution_kind_id
											       and [PostFix] = @resolution_postfix
									     ), 1));

		DECLARE @resolution_kind_reg_reject_force_id INT = @resolution_kind_id;
		DECLARE @resolution_kind_reg_reject_force_need_changing BIT = @resolution_need_changing;
	END;
END;


IF @resolution_kind_expertise_quality_need_changing = 1
   OR @resolution_kind_expertise_quality_force_need_changing = 1
   OR @resolution_kind_ethics_need_changing = 1
   OR @resolution_kind_ethics_force_quality_need_changing = 1
   OR @resolution_kind_reg_allow_need_changing = 1
   OR @resolution_kind_reg_allow_force_need_changing = 1
   OR @resolution_kind_emergency_reg_allow_need_changing = 1
   OR @resolution_kind_reg_reject_need_changing = 1
   OR @resolution_kind_reg_reject_force_need_changing = 1
BEGIN
	BEGIN TRAN;

	BEGIN TRY
		BEGIN -- backup
			DECLARE @tableName VARCHAR(100)
			SET @tableName = '[GrlsHistory].[grls].ResolutionKinds_5106_'+REPLACE(REPLACE(REPLACE(CONVERT(varchar,GETDATE(), 120), '-', '_'), ' ', '_'), ':', '_');
			IF OBJECT_ID(@tableName) IS NULL BEGIN
				EXEC('SELECT * INTO '+@tableName+' FROM grls.dbo.ResolutionKinds') 
			END
		END;


		IF @resolution_kind_expertise_quality_need_changing = 1 
			AND @resolution_kind_expertise_quality_id IS NOT NULL
		BEGIN
			UPDATE [dbo].[ResolutionKinds]
			SET [PostFix] = @resolution_kind_expertise_quality__postfix
			WHERE [ResolutionKindID] = @resolution_kind_expertise_quality_id;
		END;

		IF @resolution_kind_expertise_quality_force_need_changing = 1 
			AND @resolution_kind_expertise_quality_force_id IS NOT NULL
		BEGIN 
			UPDATE [dbo].[ResolutionKinds]
			SET [PostFix] = @resolution_kind_expertise_quality_force__postfix
			WHERE [ResolutionKindID] = @resolution_kind_expertise_quality_force_id;
		END;

		IF @resolution_kind_ethics_need_changing = 1 
			AND @resolution_kind_ethics_id IS NOT NULL
		BEGIN
			UPDATE [dbo].[ResolutionKinds]
			SET [PostFix] = @resolution_kind_ethics__postfix
			WHERE [ResolutionKindID] = @resolution_kind_ethics_id;
		END;

		IF @resolution_kind_ethics_force_quality_need_changing = 1 
			AND @resolution_kind_ethics_force_quality_id IS NOT NULL
		BEGIN
			UPDATE [dbo].[ResolutionKinds]
			SET [PostFix] = @resolution_kind_ethics_force_quality__postfix
			WHERE [ResolutionKindID] = @resolution_kind_ethics_force_quality_id;
		END;

		IF @resolution_kind_reg_allow_need_changing = 1 
			AND @resolution_kind_reg_allow_id IS NOT NULL
		BEGIN
			UPDATE [dbo].[ResolutionKinds]
			SET [PostFix] = @resolution_kind_reg_allow__postfix
			WHERE [ResolutionKindID] = @resolution_kind_reg_allow_id;
		END;

		IF @resolution_kind_reg_allow_force_need_changing = 1 
			AND @resolution_kind_reg_allow_force_id IS NOT NULL
		BEGIN
			UPDATE [dbo].[ResolutionKinds]
			SET [PostFix] = @resolution_kind_reg_allow_force__postfix
			WHERE [ResolutionKindID] = @resolution_kind_reg_allow_force_id;
		END;

		IF @resolution_kind_emergency_reg_allow_need_changing = 1 
			AND @resolution_kind_emergency_reg_allow_id IS NOT NULL
		BEGIN
			UPDATE [dbo].[ResolutionKinds]
			SET [PostFix] = @resolution_kind_emergency_reg_allow__postfix
			WHERE [ResolutionKindID] = @resolution_kind_emergency_reg_allow_id;
		END;

		IF @resolution_kind_reg_reject_need_changing = 1 
			AND @resolution_kind_reg_reject_id IS NOT NULL
		BEGIN
			UPDATE [dbo].[ResolutionKinds]
			SET [PostFix] = @resolution_kind_reg_reject__postfix
			WHERE [ResolutionKindID] = @resolution_kind_reg_reject_id;
		END;

		IF @resolution_kind_reg_reject_force_need_changing = 1 
			AND @resolution_kind_reg_reject_force_id IS NOT NULL
		BEGIN
			UPDATE [dbo].[ResolutionKinds]
			SET [PostFix] = @resolution_kind_reg_reject_force__postfix
			WHERE [ResolutionKindID] = @resolution_kind_reg_reject_force_id;
		END;

		COMMIT TRAN;
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;

		SELECT 'ERROR_NUMBER=' + CONVERT(VARCHAR(100), ISNULL(ERROR_NUMBER(), 0)) + 
		', ERROR_SEVERITY=' + CONVERT(VARCHAR(100), ISNULL(ERROR_SEVERITY(), 0)) + 
		', ERROR_STATE=' + CONVERT(VARCHAR(100), ISNULL(ERROR_STATE(), 0)) + 
		', ERROR_PROCEDURE=' + CONVERT(VARCHAR(100), ISNULL(ERROR_PROCEDURE(), 0)) + 
		', ERROR_LINE=' + CONVERT(VARCHAR(100), ISNULL(ERROR_LINE(), 0)) + 
		', ERROR_MESSAGE=' + ERROR_MESSAGE();
	END CATCH
END