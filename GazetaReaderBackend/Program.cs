using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using GazetaReaderFrontend.Services;
using Quartz;
using Quartz.Impl;


namespace GazetaReaderFrontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Настройка планировщика
            
            // Создаем планировщик с помощью factory и запускаем его.
            IScheduler sched = StdSchedulerFactory.GetDefaultScheduler();

            sched.Start();

            // Объявляем job и связываем ее с классом GetDataService.
            IJobDetail job = JobBuilder
                .Create<GetDataService>()
                .Build();

            // Создаем trigger для планировщика, задаем его расписание(начать сейчас и повторять каждые 20 минут).
            ITrigger trigger = TriggerBuilder
                .Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(20)
                .RepeatForever())
                .Build();

            // Выполняем задание по расписанию.
            sched.ScheduleJob(job, trigger);
            #endregion

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .Configure(app => app.Run(async (x)=>await x.Response.SendFileAsync("data.xml")))
                .Build();

            host.Run();

            // Остановка планировщика при остановке сервера.
            sched.Shutdown();
            
          
        }

        
    }

    
}
