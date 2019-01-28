import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-endpoints',
  templateUrl: './endpoints.component.html',
  styleUrls: ['./endpoints.component.css']
})
export class EndpointsComponent implements OnInit {
  public endpoints: Endpoint[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Endpoint[]>(baseUrl + 'api/SampleData/Endpoints').subscribe(result => {
      this.endpoints = result;
    }, error => console.error(error));
  }

  ngOnInit() {
  }

}

interface Endpoint {
  endpointName: string;
  message: string;
  environment: string;
  version: string;
  utcTimestamp: string;
  refreshIntervalInSeconds: string;
}
