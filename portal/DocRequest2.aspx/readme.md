DocRequest 

Что такое RecType в таблице dbo.[StatementText]? Версия заявления?

[На данный момент рассматривается поток регистрации]
statementTypeList: 
- applicant_request_mr
- eec_applicant_request_cp
- applicant_request_mr_fgbu
- eec_applicant_request_cp_fgbu 

DocRequest это, грубо говоря, 
- "Запросы МЗ"
- "Запросы ФГБУ"
- есть еще третий, но как мне объявняли на вводной - он не используется, Инспектирование вроде называется...)

Эти запросы делятся на подзапросы dbo.[DocSubType]. 

Если DocSubType нет (почему его не может быть?), то выбирается отчет либо rpDocRequestFS, либо rpDocRequest2
Если же DocSubType есть, то имя шаблона извлекается из БД... 

Где найти Формы ?
LetterTxt	    ИНФ
LetterInLPlTxt	ИНФ-ФГБУ
LetterInLPlTxt	ИНФфс-ФГБУ
LetterTxt_eec	ИНФ
