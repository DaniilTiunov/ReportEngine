import sys
import json
import argparse
import traceback
import PassportGenerator
import TechCardsGenerator



def main():


    try:

        parser = argparse.ArgumentParser(description='Python Script Launcher')
        parser.add_argument('--script', choices=['passport', 'techcard'], required=True, help='Script to run')
        parser.add_argument('--jsonPath', required = True, help = "Input json path")
        parser.add_argument('--outputFilePath', required = True, help = "Output file path")

    
        args = parser.parse_args()
    
        if args.script == 'passport':
           PassportGenerator.generateReport(args.jsonPath,args.outputFilePath)
        elif args.script == 'techcard':
            TechCardsGenerator.generateReport(args.jsonPath,args.outputFilePath)



        executionResult = {"Success": True, 
                           "Error": None}
        print(json.dumps(executionResult))

    except Exception as e:

        executionResult = {"Success": False, 
                           "Error": {
                                "Type": type(e).__name__,
                                "Message": str(e),
                                "Traceback": traceback.format_exc()
                               }}
        print(json.dumps(executionResult))
        sys.exit(1)
    
if __name__ == "__main__":
    main()
