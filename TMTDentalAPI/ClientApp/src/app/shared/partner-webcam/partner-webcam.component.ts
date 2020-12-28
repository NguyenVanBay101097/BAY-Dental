import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-partner-webcam',
  templateUrl: './partner-webcam.component.html',
  styleUrls: ['./partner-webcam.component.css']
})
export class PartnerWebcamComponent implements OnInit {

  @ViewChild('video', { static: true }) public video: ElementRef;

  @ViewChild("canvas", { static: true }) public canvas: ElementRef;

  imageUrl: any;
  hasDevice = true;

  constructor(private activeModal: NgbActiveModal) { }

  ngOnInit() {
  }

  public ngAfterViewInit() {
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
      navigator.mediaDevices.getUserMedia({ video: true }).then(resolve => {
        this.hasDevice = true;
        this.video.nativeElement.src = window.URL.createObjectURL(resolve);
        this.video.nativeElement.play();
      }, (reject) => {
        this.hasDevice = false;
      });
    }
  }

  onSnap() {
    var context = this.canvas.nativeElement.getContext("2d").drawImage(this.video.nativeElement, 0, 0, 640, 480);
    this.imageUrl = this.canvas.nativeElement.toDataURL("image/png");
  }

  onSave() {
    var blobBin = atob(this.imageUrl.split(',')[1]);
    var array = [];
    for (var i = 0; i < blobBin.length; i++) {
      array.push(blobBin.charCodeAt(i));
    }
    var file = new Blob([new Uint8Array(array)], { type: 'image/png' });
    this.activeModal.close(file);
  }


}
