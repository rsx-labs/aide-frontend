@echo off
"C:\Program Files (x86)\kool-aide\kool-aide" generate-report -r status-report --format excel --output "C:\GeneratedReports\DEV Weekly Status Report.xlsx" --params {\"weeks\":[%1]}

