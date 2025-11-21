import sys
import argparse
import PassportGenerator
import TechCardsGenerator


def main():

    parser = argparse.ArgumentParser(description='Python Script Launcher')
    parser.add_argument('--script', choices=['passport', 'techcard'], required=True, help='Script to run')
    parser.add_argument('--jsonPath', required = True, help = "Input json path")
    parser.add_argument('--outputReportPath', required = True, help = "Output report path")

    
    args = parser.parse_args()
    
    if args.script == 'passport':
        PassportGenerator.generateReport(args.jsonPath,args.outputReportPath)
    elif args.script == 'techcard':
        TechCardsGenerator.generateReport(args.jsonPath,args.outputReportPath)
    
if __name__ == "__main__":
    main()
