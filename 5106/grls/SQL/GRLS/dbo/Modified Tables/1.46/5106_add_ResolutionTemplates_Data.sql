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


BEGIN -- проверка
	DECLARE @resolution_kind_code NVARCHAR(100) = NULL,
			@template_id INT = NULL,
			@template_need_change BIT = NULL,
			@template__name NVARCHAR(500) = NULL,
			@template__result NVARCHAR(MAX) = NULL,
			@template__additional NVARCHAR(MAX) = NULL;

	BEGIN -- resolution_mr_em_q_riskexp_req
		SET @resolution_kind_code = NULL;
		SET @template_id = NULL;
		SET @template_need_change = 1;
		
		DECLARE 
				@template_resolution_expertise__name NVARCHAR(500) = N'Решение о проведении экспертизы лекарственного средства для медицинского применения в части экспертизы качества лекарственного средства и экспертизы отношения ожидаемой пользы к возможному риску применения лекарственного препарата для медицинского применения',
				@template_resolution_expertise__result NVARCHAR(MAX) = N'       Министерством здравоохранения Российской Федерации в соответствии с постановлением Правительства РФ от 05.04.2022 № 593 "Об особенностях обращения лекарственных средств для медицинского применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов в связи с введением в отношении Российской Федерации ограничительных мер экономического характера" (далее – Постановление) на основании представленного заявления № [idStatement] (вх. № [inNumber] от [inDate]) и документов принято решение о проведении экспертизы лекарственного средства для медицинского применения в части экспертизы качества лекарственного средства и экспертизы отношения ожидаемой пользы к возможному риску применения лекарственного препарата для медицинского применения:',
				@template_resolution_expertise__additional NVARCHAR(MAX) = N'       Одновременно сообщаем, что в соответствии с Постановлением   необходимо представить в ФГБУ "НЦЭСМП" Минздрава России образцы лекарственного препарата для медицинского применения, а также образец фармацевтической субстанции и образцы веществ, применяемых для контроля качества лекарственного средства путем сравнения с ними исследуемого лекарственного средства, в количествах, необходимых для воспроизведения методов контроля качества.';
	
		SET @resolution_kind_code = @RESOLUTION_KIND_EXPERTISE_QUALITY_CODE;
		
		SET @template__name = @template_resolution_expertise__name;
		SET @template__result = @template_resolution_expertise__result;
		SET @template__additional = @template_resolution_expertise__additional;

		SET @template_id = (select top(1) [templates].[ResolutionTemplateId] 
							from [dbo].[ResolutionKinds] [kinds]
									inner join [dbo].[ResolutionTemplates] [templates] on [kinds].[ResolutionTemplateId] = [templates].[ResolutionTemplateID]
							where [kinds].[Code] = @resolution_kind_code
							  and [kinds].[StatementTypeId] = @defect_statement_type_id);
		SET @template_need_change = (isNull((select 0
									 from [dbo].[ResolutionTemplates] [templates]
									 where [templates].[ResolutionTemplateId] = @template_id
									   and [templates].[Name] = @template__name
									   and [templates].[Additional] = @template__additional
									   and [templates].[Result] = @template__result
									 ), 1));

	
		DECLARE @template_resolution_expertise_id INT = @template_id;
		DECLARE @template_resolution_expertise_need_chagne BIT = @template_need_change;
	END
	BEGIN -- resolution_mr_em_q_riskexp_req_force
		SET @resolution_kind_code = NULL;
		SET @template_id = NULL;
		SET @template_need_change = 1;

		DECLARE 
				@template_resolution_expertise_force__name NVARCHAR(500) = N'Решение о проведении экспертизы лекарственного средства для медицинского применения в части экспертизы качества лекарственного средства и экспертизы отношения ожидаемой пользы к возможному риску применения лекарственного препарата для медицинского применения',
				@template_resolution_expertise_force__result NVARCHAR(MAX) = N'       Министерством здравоохранения Российской Федерации в соответствии с постановлением Правительства РФ от 05.04.2022 № 593 "Об особенностях обращения лекарственных средств для медицинского применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов в связи с введением в отношении Российской Федерации ограничительных мер экономического характера" (далее – Постановление) на основании представленного заявления № [idStatement] (вх. № [inNumber] от [inDate]) и документов принято решение о проведении экспертизы лекарственного средства для медицинского применения в части экспертизы качества лекарственного средства и экспертизы отношения ожидаемой пользы к возможному риску применения лекарственного препарата для медицинского применения:',
				@template_resolution_expertise_force__additional NVARCHAR(MAX) = N'       Одновременно сообщаем, что в соответствии с Постановлением   необходимо представить в ФГБУ "НЦЭСМП" Минздрава России образцы лекарственного препарата для медицинского применения, а также образец фармацевтической субстанции и образцы веществ, применяемых для контроля качества лекарственного средства путем сравнения с ними исследуемого лекарственного средства, в количествах, необходимых для воспроизведения методов контроля качества.';
		
		SET @resolution_kind_code = @RESOLUTION_KIND_EXPERTISE_QUALITY_FORCE_CODE;
		
		SET @template__name = @template_resolution_expertise_force__name;
		SET @template__result = @template_resolution_expertise_force__result;
		SET @template__additional = @template_resolution_expertise_force__additional;

		SET @template_id = (select top(1) [templates].[ResolutionTemplateId] 
							from [dbo].[ResolutionKinds] [kinds]
									inner join [dbo].[ResolutionTemplates] [templates] on [kinds].[ResolutionTemplateId] = [templates].[ResolutionTemplateID]
							where [kinds].[Code] = @resolution_kind_code
							  and [kinds].[StatementTypeId] = @defect_statement_type_id);
		SET @template_need_change = (isNull((select 0
									 from [dbo].[ResolutionTemplates] [templates]
									 where [templates].[ResolutionTemplateId] = @template_id
									   and [templates].[Name] = @template__name
									   and [templates].[Additional] = @template__additional
									   and [templates].[Result] = @template__result
									 ), 1));

	
		DECLARE @template_resolution_expertise_force_id INT = @template_id;
		DECLARE @template_resolution_expertise_force_need_chagne BIT = @template_need_change;
	END
	BEGIN -- resolution_mr_em_etexp_req
		SET @resolution_kind_code = NULL;
		SET @template_id = NULL;
		SET @template_need_change = 1;

		DECLARE 
				@template_resolution_ethic__name NVARCHAR(500) = N'Решение о проведении этической экспертизы связанных с установлением возможности государственной регистрации лекарственного препарата, который предназначен для применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов',
				@template_resolution_ethic__result NVARCHAR(MAX) = N'       Министерство здравоохранения Российской Федерации в соответствии с постановлением Правительства Российской Федерации от 05.04.2022 № 593 "Об особенностях обращения лекарственных средств для медицинского применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов в связи с введением в отношении Российской Федерации ограничительных мер экономического характера"  (далее – Постановление) на основании представленного заявления № [idStatement] (вх. № [inNumber] от [inDate]) направляет на проведение этической экспертизы документы, связанные с установлением возможности государственной регистрации лекарственного препарата:',
				@template_resolution_ethic__additional NVARCHAR(MAX) = N'';
		
		SET @resolution_kind_code = @RESOLUTION_KIND_ETHICS_CODE;

		SET @template__name = @template_resolution_ethic__name;
		SET @template__result = @template_resolution_ethic__result;
		SET @template__additional = @template_resolution_ethic__additional;

		SET @template_id = (select top(1) [templates].[ResolutionTemplateId] 
							from [dbo].[ResolutionKinds] [kinds]
									inner join [dbo].[ResolutionTemplates] [templates] on [kinds].[ResolutionTemplateId] = [templates].[ResolutionTemplateID]
							where [kinds].[Code] = @resolution_kind_code
							  and [kinds].[StatementTypeId] = @defect_statement_type_id);
		SET @template_need_change = (isNull((select 0
									 from [dbo].[ResolutionTemplates] [templates]
									 where [templates].[ResolutionTemplateId] = @template_id
									   and [templates].[Name] = @template__name
									   and [templates].[Additional] = @template__additional
									   and [templates].[Result] = @template__result
									 ), 1));

	
		DECLARE @template_resolution_ethic_id INT = @template_id;
		DECLARE @template_resolution_ethic_need_change BIT = @template_need_change;
	END
	BEGIN -- resolution_mr_em_etexp_req_force
		SET @resolution_kind_code = NULL;
		SET @template_id = NULL;
		SET @template_need_change = 1;

		DECLARE 
				@template_resolution_ethic_force__name NVARCHAR(500) = N'Решение о проведении этической экспертизы связанных с установлением возможности государственной регистрации лекарственного препарата, который предназначен для применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов',
				@template_resolution_ethic_force__result NVARCHAR(MAX) = N'       Министерство здравоохранения Российской Федерации в соответствии с постановлением Правительства Российской Федерации от 05.04.2022 № 593 "Об особенностях обращения лекарственных средств для медицинского применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов в связи с введением в отношении Российской Федерации ограничительных мер экономического характера"  (далее – Постановление) на основании представленного заявления № [idStatement] (вх. № [inNumber] от [inDate]) направляет на проведение этической экспертизы документы, связанные с установлением возможности государственной регистрации лекарственного препарата:',
				@template_resolution_ethic_force__additional NVARCHAR(MAX) = N'';
		
		SET @resolution_kind_code = @RESOLUTION_KIND_ETHICS_FORCE_CODE;


		SET @template__name = @template_resolution_ethic_force__name;
		SET @template__result = @template_resolution_ethic_force__result;
		SET @template__additional = @template_resolution_ethic_force__additional;

		SET @template_id = (select top(1) [templates].[ResolutionTemplateId] 
							from [dbo].[ResolutionKinds] [kinds]
									inner join [dbo].[ResolutionTemplates] [templates] on [kinds].[ResolutionTemplateId] = [templates].[ResolutionTemplateID]
							where [kinds].[Code] = @resolution_kind_code
							  and [kinds].[StatementTypeId] = @defect_statement_type_id);
		SET @template_need_change = (isNull((select 0
									 from [dbo].[ResolutionTemplates] [templates]
									 where [templates].[ResolutionTemplateId] = @template_id
									   and [templates].[Name] = @template__name
									   and [templates].[Additional] = @template__additional
									   and [templates].[Result] = @template__result
									 ), 1));

	
		DECLARE @template_resolution_ethic_force_id INT = @template_id;
		DECLARE @template_resolution_ethic_force_need_change BIT = @template_need_change;
	END
	BEGIN -- resolution_mr_em_allow_req
		SET @resolution_kind_code = NULL;
		SET @template_id = NULL;
		SET @template_need_change = 1;

		DECLARE 
				@template_resolution_allow__name NVARCHAR(500) = N'Решение о государственной регистрации лекарственного препарата для медицинского применения',
				@template_resolution_allow__result NVARCHAR(MAX) = N'       Министерством здравоохранения Российской в соответствии с постановлением Правительства Российской Федерации от 05.04.2022 № 593 "Об особенностях обращения лекарственных средств для медицинского применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов в связи с введением в отношении Российской Федерации ограничительных мер экономического характера" (далее – Постановление) и на основании представленного заявления о государственной регистрации лекарственного препарата и регистрационного досье (вх. № [inNumber] от [inDate]), по результатам проведенных экспертиз принято решение о государственной регистрации лекарственного препарата для медицинского применения:',
				@template_resolution_allow__additional NVARCHAR(MAX) = N'[RefusalReason]';
		
		SET @resolution_kind_code = @RESOLUTION_KIND_REG_ALLOW_CODE;


		SET @template__name = @template_resolution_allow__name;
		SET @template__result = @template_resolution_allow__result;
		SET @template__additional = @template_resolution_allow__additional;

		SET @template_id = (select top(1) [templates].[ResolutionTemplateId] 
							from [dbo].[ResolutionKinds] [kinds]
									inner join [dbo].[ResolutionTemplates] [templates] on [kinds].[ResolutionTemplateId] = [templates].[ResolutionTemplateID]
							where [kinds].[Code] = @resolution_kind_code
							  and [kinds].[StatementTypeId] = @defect_statement_type_id);
		SET @template_need_change = (isNull((select 0
									 from [dbo].[ResolutionTemplates] [templates]
									 where [templates].[ResolutionTemplateId] = @template_id
									   and [templates].[Name] = @template__name
									   and [templates].[Additional] = @template__additional
									   and [templates].[Result] = @template__result
									 ), 1));

	
		DECLARE @template_resolution_allow_id INT = @template_id;
		DECLARE @template_resolution_allow_need_change BIT = @template_need_change;
	END
	BEGIN -- resolution_mr_em_allow_req_force
		SET @resolution_kind_code = NULL;
		SET @template_id = NULL;
		SET @template_need_change = 1;

		DECLARE 
				@template_resolution_allow_force__name NVARCHAR(500) = N'Решение о государственной регистрации лекарственного препарата для медицинского применения',
				@template_resolution_allow_force__result NVARCHAR(MAX) = N'       Министерством здравоохранения Российской в соответствии с постановлением Правительства Российской Федерации от 05.04.2022 № 593 "Об особенностях обращения лекарственных средств для медицинского применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов в связи с введением в отношении Российской Федерации ограничительных мер экономического характера" (далее – Постановление) и на основании представленного заявления о государственной регистрации лекарственного препарата и регистрационного досье (вх. № [inNumber] от [inDate]), по результатам проведенных экспертиз принято решение о государственной регистрации лекарственного препарата для медицинского применения:',
				@template_resolution_allow_force__additional NVARCHAR(MAX) = N'[RefusalReason]';
		
		SET @resolution_kind_code = @RESOLUTION_KIND_REG_FORCE_ALLOW_CODE;


		SET @template__name = @template_resolution_allow_force__name;
		SET @template__result = @template_resolution_allow_force__result;
		SET @template__additional = @template_resolution_allow_force__additional;

		SET @template_id = (select top(1) [templates].[ResolutionTemplateId] 
							from [dbo].[ResolutionKinds] [kinds]
									inner join [dbo].[ResolutionTemplates] [templates] on [kinds].[ResolutionTemplateId] = [templates].[ResolutionTemplateID]
							where [kinds].[Code] = @resolution_kind_code
							  and [kinds].[StatementTypeId] = @defect_statement_type_id);
		SET @template_need_change = (isNull((select 0
									 from [dbo].[ResolutionTemplates] [templates]
									 where [templates].[ResolutionTemplateId] = @template_id
									   and [templates].[Name] = @template__name
									   and [templates].[Additional] = @template__additional
									   and [templates].[Result] = @template__result
									 ), 1));

	
		DECLARE @template_resolution_allow_force_id INT = @template_id;
		DECLARE @template_resolution_allow_force_need_change BIT = @template_need_change;
	END
	BEGIN -- resolution_mr_emergency_em_allow_req
		SET @resolution_kind_code = NULL;
		SET @template_id = NULL;
		SET @template_need_change = 1;

		DECLARE 
				@template_resolution_emergency_allow_force__name NVARCHAR(500) = N'Решение о государственной регистрации лекарственного препарата для медицинского применения',
				@template_resolution_emergency_allow_force__result NVARCHAR(MAX) = N'       Министерством здравоохранения Российской в соответствии с постановлением Правительства Российской Федерации от 05.04.2022 № 593 "Об особенностях обращения лекарственных средств для медицинского применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов в связи с введением в отношении Российской Федерации ограничительных мер экономического характера" (далее – Постановление) и на основании представленного заявления о государственной регистрации лекарственного препарата и регистрационного досье (вх. № [inNumber] от [inDate]), по результатам проведенных экспертиз принято решение о государственной регистрации лекарственного препарата для медицинского применения:',
				@template_resolution_emergency_allow_force__additional NVARCHAR(MAX) = N'[RefusalReason]';
		
		SET @resolution_kind_code = @RESOLUTION_KIND_EMERGENCY_REG_ALLOW_CODE;

		SET @template__name = @template_resolution_emergency_allow_force__name;
		SET @template__result = @template_resolution_emergency_allow_force__result;
		SET @template__additional = @template_resolution_emergency_allow_force__additional;

		SET @template_id = (select top(1) [templates].[ResolutionTemplateId] 
							from [dbo].[ResolutionKinds] [kinds]
									inner join [dbo].[ResolutionTemplates] [templates] on [kinds].[ResolutionTemplateId] = [templates].[ResolutionTemplateID]
							where [kinds].[Code] = @resolution_kind_code
							  and [kinds].[StatementTypeId] = @defect_statement_type_id);
		SET @template_need_change = (isNull((select 0
									 from [dbo].[ResolutionTemplates] [templates]
									 where [templates].[ResolutionTemplateId] = @template_id
									   and [templates].[Name] = @template__name
									   and [templates].[Additional] = @template__additional
									   and [templates].[Result] = @template__result
									 ), 1));

	
		DECLARE @template_resolution_emergency_allow_force_id INT = @template_id;
		DECLARE @template_resolution_emergency_allow_force_need_change BIT = @template_need_change;
	END
	BEGIN -- resolution_mr_emergency_reject_fail_req
		SET @resolution_kind_code = NULL;
		SET @template_id = NULL;
		SET @template_need_change = 1;

		DECLARE 
				@template_resolution_emergency_reject__name NVARCHAR(500) = N'Решение об отказе в государственной регистрации лекарственного препарата для медицинского применения',
				@template_resolution_emergency_reject__result NVARCHAR(MAX) = N'       Министерством здравоохранения Российской Федерации в соответствии с постановлением Правительства Российской Федерации от 05.04.2022 № 593 "Об особенностях обращения лекарственных средств для медицинского применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов в связи с введением в отношении Российской Федерации ограничительных мер экономического характера" (далее – Постановление) на основании представленного заявления № [idStatement] (вх. № [inNumber] от [inDate]) и документов принято решение об отказе в государственной регистрации лекарственного препарата для медицинского применения:',
				@template_resolution_emergency_reject__additional NVARCHAR(MAX) = N'       Основанием для отказа в государственной регистрации лекарственного препарата для медицинского применения в соответствии с пунктом 19 Постановления является [RefusalReason]';
		
		SET @resolution_kind_code = @RESOLUTION_KIND_REG_REJECT_CODE;

		SET @template__name = @template_resolution_emergency_reject__name;
		SET @template__result = @template_resolution_emergency_reject__result;
		SET @template__additional = @template_resolution_emergency_reject__additional;

		SET @template_id = (select top(1) [templates].[ResolutionTemplateId] 
							from [dbo].[ResolutionKinds] [kinds]
									inner join [dbo].[ResolutionTemplates] [templates] on [kinds].[ResolutionTemplateId] = [templates].[ResolutionTemplateID]
							where [kinds].[Code] = @resolution_kind_code
							  and [kinds].[StatementTypeId] = @defect_statement_type_id);
		SET @template_need_change = (isNull((select 0
									 from [dbo].[ResolutionTemplates] [templates]
									 where [templates].[ResolutionTemplateId] = @template_id
									   and [templates].[Name] = @template__name
									   and [templates].[Additional] = @template__additional
									   and [templates].[Result] = @template__result
									 ), 1));

	
		DECLARE @template_resolution_emergency_reject_id INT = @template_id;
		DECLARE @template_resolution_emergency_reject_need_change BIT = @template_need_change;
	END
	BEGIN -- resolution_mr_emergency_reject_fail_force__req
		SET @resolution_kind_code = NULL;
		SET @template_id = NULL;
		SET @template_need_change = 1;

		DECLARE 
				@template_resolution_emergency_reject_force__name NVARCHAR(500) = N'Решение об отказе в государственной регистрации лекарственного препарата для медицинского применения',
				@template_resolution_emergency_reject_force__result NVARCHAR(MAX) = N'       Министерством здравоохранения Российской Федерации в соответствии с постановлением Правительства Российской Федерации от 05.04.2022 № 593 "Об особенностях обращения лекарственных средств для медицинского применения в случае дефектуры или риска возникновения дефектуры лекарственных препаратов в связи с введением в отношении Российской Федерации ограничительных мер экономического характера" (далее – Постановление) на основании представленного заявления № [idStatement] (вх. № [inNumber] от [inDate]) и документов принято решение об отказе в государственной регистрации лекарственного препарата для медицинского применения:',
				@template_resolution_emergency_reject_force__additional NVARCHAR(MAX) = N'       Основанием для отказа в государственной регистрации лекарственного препарата для медицинского применения в соответствии с пунктом 19 Постановления является [RefusalReason]';
		
		SET @resolution_kind_code = @RESOLUTION_KIND_REG_REJECT_FORCE_CODE

		SET @template__name = @template_resolution_emergency_reject_force__name;
		SET @template__result = @template_resolution_emergency_reject_force__result;
		SET @template__additional = @template_resolution_emergency_reject_force__additional;

		SET @template_id = (select top(1) [templates].[ResolutionTemplateId] 
							from [dbo].[ResolutionKinds] [kinds]
									inner join [dbo].[ResolutionTemplates] [templates] on [kinds].[ResolutionTemplateId] = [templates].[ResolutionTemplateID]
							where [kinds].[Code] = @resolution_kind_code
							  and [kinds].[StatementTypeId] = @defect_statement_type_id);
		SET @template_need_change = (isNull((select 0
									 from [dbo].[ResolutionTemplates] [templates]
									 where [templates].[ResolutionTemplateId] = @template_id
									   and [templates].[Name] = @template__name
									   and [templates].[Additional] = @template__additional
									   and [templates].[Result] = @template__result
									 ), 1));

	
		DECLARE @template_resolution_emergency_reject_force_id INT = @template_id;
		DECLARE @template_resolution_emergency_reject_force_need_change BIT = @template_need_change;
	END
END;

IF @template_resolution_expertise_need_chagne = 1
   OR @template_resolution_expertise_force_need_chagne = 1
   OR @template_resolution_ethic_need_change = 1
   OR @template_resolution_ethic_force_need_change = 1
   OR @template_resolution_allow_need_change = 1
   OR @template_resolution_allow_force_need_change = 1
   OR @template_resolution_emergency_allow_force_need_change = 1
   OR @template_resolution_emergency_reject_need_change = 1
   OR @template_resolution_emergency_reject_force_need_change = 1
BEGIN
	BEGIN TRAN;

	BEGIN TRY
		BEGIN -- backup
			DECLARE @tableName VARCHAR(100)
			SET @tableName = '[GrlsHistory].[grls].ResolutionTemplates_5106_'+REPLACE(REPLACE(REPLACE(CONVERT(varchar,GETDATE(), 120), '-', '_'), ' ', '_'), ':', '_');
			IF OBJECT_ID(@tableName) IS NULL BEGIN
				EXEC('SELECT * INTO '+@tableName+' FROM grls.dbo.ResolutionTemplates') 
			END
		END;
	
		BEGIN -- ЭКфПР
			IF @template_resolution_expertise_need_chagne = 1
			   AND @template_resolution_expertise_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionTemplates]
				SET [Name] = @template_resolution_expertise__name,
					[Result] = @template_resolution_expertise__result,
					[Additional] = @template_resolution_expertise__additional
				WHERE [ResolutionTemplateID] = @template_resolution_expertise_id
			END;
		END
		BEGIN -- ЭКфПР (force)
			IF @template_resolution_expertise_force_need_chagne = 1 
				AND @template_resolution_expertise_force_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionTemplates]
				SET [Name] = @template_resolution_expertise_force__name,
					[Result] = @template_resolution_expertise_force__result,
					[Additional] = @template_resolution_expertise_force__additional
				WHERE [ResolutionTemplateID] = @template_resolution_expertise_force_id
			END;
		END
		BEGIN -- ЭТ
			IF @template_resolution_ethic_need_change = 1 
				AND @template_resolution_ethic_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionTemplates]
				SET [Name] = @template_resolution_ethic__name,
					[Result] = @template_resolution_ethic__result,
					[Additional] = @template_resolution_ethic__additional
				WHERE [ResolutionTemplateID] = @template_resolution_ethic_id
			END;
		END
		BEGIN -- ЭТ (force)
			IF @template_resolution_ethic_force_need_change = 1 
				AND @template_resolution_ethic_force_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionTemplates]
				SET [Name] = @template_resolution_ethic_force__name,
					[Result] = @template_resolution_ethic_force__result,
					[Additional] = @template_resolution_ethic_force__additional
				WHERE [ResolutionTemplateID] = @template_resolution_ethic_force_id
			END;
		END
		BEGIN -- РЛП
			IF @template_resolution_allow_need_change = 1 
				AND @template_resolution_allow_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionTemplates]
				SET [Name] = @template_resolution_allow__name,
					[Result] = @template_resolution_allow__result,
					[Additional] = @template_resolution_allow__additional
				WHERE [ResolutionTemplateID] = @template_resolution_allow_id
			END;
		END
		BEGIN -- РЛП (force)
			IF @template_resolution_allow_force_need_change = 1 
				AND @template_resolution_allow_force_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionTemplates]
				SET [Name] = @template_resolution_allow_force__name,
					[Result] = @template_resolution_allow_force__result,
					[Additional] = @template_resolution_allow_force__additional
				WHERE [ResolutionTemplateID] = @template_resolution_allow_force_id
			END;
		END
		BEGIN -- РЛП (emergency)
			IF @template_resolution_emergency_allow_force_need_change = 1 
			   AND @template_resolution_emergency_allow_force_id IS NOT NULL 
			BEGIN
				UPDATE [dbo].[ResolutionTemplates]
				SET [Name] = @template_resolution_emergency_allow_force__name,
					[Result] = @template_resolution_emergency_allow_force__result,
					[Additional] = @template_resolution_emergency_allow_force__additional
				WHERE [ResolutionTemplateID] = @template_resolution_emergency_allow_force_id
			END;
		END
		BEGIN -- РЛПО
			IF @template_resolution_emergency_reject_need_change = 1 
				AND @template_resolution_emergency_reject_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionTemplates]
				SET [Name] = @template_resolution_emergency_reject__name,
					[Result] = @template_resolution_emergency_reject__result,
					[Additional] = @template_resolution_emergency_reject__additional
				WHERE [ResolutionTemplateID] = @template_resolution_emergency_reject_id
			END;
		END
		BEGIN -- РЛПО (force)
			IF @template_resolution_emergency_reject_force_need_change = 1 
				AND @template_resolution_emergency_reject_force_id IS NOT NULL
			BEGIN
				UPDATE [dbo].[ResolutionTemplates]
				SET [Name] = @template_resolution_emergency_reject_force__name,
					[Result] = @template_resolution_emergency_reject_force__result,
					[Additional] = @template_resolution_emergency_reject_force__additional
				WHERE [ResolutionTemplateID] = @template_resolution_emergency_reject_force_id
			END;
		END
	
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
