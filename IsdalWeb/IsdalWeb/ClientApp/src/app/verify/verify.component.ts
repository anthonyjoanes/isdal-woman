import { Component, Inject, EventEmitter } from '@angular/core';
import { FileUploader, FileLikeObject } from 'ng2-file-upload';
import { HttpClient } from '@angular/common/http';

class TestImage {
  id: number;
  name: string;
  description: string;
  selected: boolean = false;
  url: string;
  result: number = 0
}

@Component({
  selector: 'app-verify-component',
  templateUrl: './verify.component.html',
  styleUrls: ['./verify.component.css']
})
export class VerifyComponent {
  public currentCount = 0;
  public selectedImage = 0;
  public testImages: TestImage[] = [];
  public uploader: FileUploader;
  public hasBaseDropZoneOver: boolean = false;
  public hasAnotherDropZoneOver: boolean = false;
  public httpClient: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;

    this.uploader = new FileUploader({
      url: `${baseUrl}api/SampleData/Verify`,
      disableMultipart: true
    });

    this.testImages.push({
      id: 0,
      name: 'Isdal woman sketch from 2017',
      description: 'Modern day replication',
      selected: false,
      url: '../../assets/isdal-woman-sketch.png',
      result: 0
    });

    this.testImages.push({
      id: 1,
      name: 'Isdal Sketch 2017',
      description: 'Darker hair flowing',
      selected: false,
      url: '../../assets/sketch-hair-down.jpg',
      result: 0
    });

    this.testImages.push({
      id: 2,
      name: 'Isdal Side Sketch',
      description: 'Another sketch but side on this time',
      selected: false,
      url: '../../assets/sketch-side.jpg',
      result: 0
    });

    this.testImages.push({
      id: 3,
      name: 'Isdal woman sketch from 2017',
      description: 'An original sketch from 1970s',
      selected: false,
      url: '../../assets/sketch-1970.jpg',
      result: 0
    });
  }

  public selectImage(imageId: number) {
    this.testImages[imageId].selected = !this.testImages[imageId].selected;
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  public fileOverAnother(e: any): void {
    this.hasAnotherDropZoneOver = e;
  }

  public onFileSelected(event: EventEmitter<File[]>) {
    const file: File = event[0];

    console.log(this.uploader.queue);

    //this.uploader.uploadItem(this.uploader.queue[0]);
    this.uploader.uploadAll();

    this.uploader.onCompleteItem = (item, response, status, header) => {
      if (status === 200) {
        console.log(response);
      }
    }

    this.readBase64(file)
      .then(function (data) {
        console.log(data);

        

        //this.httpClient.post(this.baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
        //  this.forecasts = result;
        //}, error => console.error(error));

      });
  }

  /**
  * 
  */
  readBase64 = (file) => {
    var reader = new FileReader();
    var future = new Promise((resolve, reject) => {
      reader.addEventListener("load", function () {
        resolve(reader.result);
      }, false);

      reader.addEventListener("error", function (event) {
        reject(event);
      }, false);

      reader.readAsDataURL(file);
    });
    return future;
  }
}
