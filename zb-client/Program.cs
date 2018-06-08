﻿using System;
using System.Collections.Generic;

namespace zbclient
{
    class MainClass
    {
        static void Main(string[] args)
        {
            ZeebeClient zeebeClient = new ZeebeClient("0.0.0.0:51015");

            JobClient jobClient = zeebeClient.DefaultTopic
                                             .JobClient("thisWorker", "myType");
            
            IList<zbclient.JobClient.Job> jobs = jobClient.Poll(4);

            Console.WriteLine("Polled {0} jobs", jobs.Count);
            foreach (zbclient.JobClient.Job job in jobs)
            {
                Console.WriteLine("Complete Job {0}", job.jobKey);
                jobClient.Complete(job.jobKey, "{}");
            }

            while (true) ;
        }

    }
}
