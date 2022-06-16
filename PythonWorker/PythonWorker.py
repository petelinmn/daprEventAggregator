import requests
import asyncio



async def main():
    while True:
        url = 'http://localhost:5005/actors/WorkerManagerActor/WorkerManagerActor/method/StartNext'
        print(11)
        response = requests.put(url, data = {'implementations':['Sleep']})
        print(22)
        if response.status_code != 200:
            await asyncio.sleep(1)
            continue

        workerArgs = response.json()

        workerId = workerArgs['workerId']
        arguments = workerArgs['args']
        workerName = arguments[0];

        print(workerName)

        if workerName == 'Sleep':
            await SleepWorker.Run(workerId, arguments)

class SleepWorker:
    async def Run(workerId, args):
        print('Start worker Name: SleepWorker,\tId: ' + workerId)
        for i in range(10):
            await asyncio.sleep(1)
            print('Worker: ' + args[0] + '\tprogress: ' + str((i+1)*10) + '%')

SleepWorker.Run = staticmethod(SleepWorker.Run)



asyncio.run(main())