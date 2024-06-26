﻿using System.Diagnostics;
using Prometheus;

namespace GameFrameX.Monitor;

public static class MetricsHelper
{
    public static void Start()
    {
        Metrics.SuppressDefaultMetrics(new SuppressDefaultMetricOptions
        {
            SuppressEventCounters = true,
            SuppressMeters = true,
            SuppressProcessMetrics = true
        });
        using var server = new KestrelMetricServer(29090);
        server.Start();
        
        // Generate some sample data from fake business logic.
        var recordsProcessed = Prometheus.Metrics.CreateCounter("sample_records_processed_total", "Total number of records processed.");
        var recordSizeInPages = Prometheus.Metrics.CreateHistogram("sample_record_size_pages", "Size of a record, in pages.", new HistogramConfiguration
        {
            Buckets = Histogram.PowersOfTenDividedBuckets(0, 2, 10)
        });

// SAMPLED EXEMPLAR: For the next histogram we only want to record exemplars for values larger than 0.1 (i.e. when record processing goes slowly).
        static Exemplar RecordExemplarForSlowRecordProcessingDuration(Collector metric, double value)
        {
            if (value < 0.1)
                return Exemplar.None;

            return Exemplar.FromTraceContext();
        }

        var recordProcessingDuration = Prometheus.Metrics.CreateHistogram("sample_record_processing_duration_seconds", "How long it took to process a record, in seconds.", new HistogramConfiguration
        {
            Buckets = Histogram.PowersOfTenDividedBuckets(-4, 1, 5),
            ExemplarBehavior = new()
            {
                DefaultExemplarProvider = RecordExemplarForSlowRecordProcessingDuration
            }
        });

        var totalSleepTime = Prometheus.Metrics.CreateCounter("sample_sleep_seconds_total", "Total amount of time spent sleeping.");

        // CUSTOM EXEMPLAR: The key from an exemplar key-value pair should be created once and reused to minimize memory allocations.
        var recordIdKey = Exemplar.Key("record_id");

        _ = Task.Run(async delegate
        {
            while (true)
            {
                // DEFAULT EXEMPLAR: We expose the trace_id and span_id for distributed tracing, based on Activity.Current.
                // Activity.Current is often automatically inherited from incoming HTTP requests if using OpenTelemetry tracing with ASP.NET Core.
                // Here, we manually create and start an activity for sample purposes, without relying on the platform managing the activity context.
                // See https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-concepts
                using (var activity = new Activity("Pausing before record processing").Start())
                {
                    var sleepStopwatch = Stopwatch.StartNew();
                    await Task.Delay(TimeSpan.FromSeconds(1));

                    // The trace_id and span_id from the current Activity are exposed as the exemplar by default.
                    totalSleepTime.Inc(sleepStopwatch.Elapsed.TotalSeconds);
                }

                using var processingDurationTimer = recordProcessingDuration.NewTimer();

                // Pretend to process a record approximately every second, just for changing sample data.
                var recordId = Guid.NewGuid();
                var recordPageCount = Random.Shared.Next(minValue: 5, maxValue: 100);

                // CUSTOM EXEMPLAR: We pass the record ID key-value pair when we increment the metric.
                // When the metric data is published to Prometheus, the most recent record ID will be attached to it.
                var exemplar = Exemplar.From(recordIdKey.WithValue(recordId.ToString()));

                // Note that one Exemplar object can only be used once. You must clone it to reuse it.
                recordsProcessed.Inc(exemplar.Clone());
                recordSizeInPages.Observe(recordPageCount, exemplar);
            }
        });
        Console.WriteLine("Open http://localhost:29090/metrics?accept=application/openmetrics-text in a web browser.");
    }
}