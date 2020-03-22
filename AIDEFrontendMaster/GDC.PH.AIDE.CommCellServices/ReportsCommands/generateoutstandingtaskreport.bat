@echo on
set "_names=%~1"
"C:\Program Files (x86)\kool-aide\kool-aide" generate-report -r task-report --format excel --output "C:\GeneratedReports\Outstanding Task Report.xlsx" --params {\"status\":[2,5,6],\"ids\":["%_names%"],\"startdate\":\"%2\"}

