import { Component, OnInit, ViewChild, ElementRef, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-binary-file-input',
  templateUrl: './binary-file-input.component.html',
  styleUrls: ['./binary-file-input.component.css']
})
export class BinaryFileInputComponent implements OnInit {
  maxUploadSize = 5 * 1024 * 1024;
  @Input() value: string;
  @Output('valueChange') change = new EventEmitter<string>();
  constructor() { }

  ngOnInit() {
  }

  onChange(ev) {
    var inputFile = ev.target;
    if (inputFile.files && inputFile.files[0]) {
      var file = inputFile.files[0];
      if (file.size > this.maxUploadSize) {
        alert("File không được vượt quá 5Mb.");
        return false;
      }
      var reader = new FileReader();
      reader.readAsDataURL(file);
      var self = this;
      reader.onloadend = function (upload) {
        var data = reader.result.toString();
        data = data.split(',')[1];
        self.onFileUploaded(file.size, file.name, file.type, data);
      };
    }
  }

  onFileUploaded(size, name, content_type, file_base64) {
    if (size === false) {
      alert("There was a problem while uploading your file");
    } else {
      this.onFileUploadedAndValid.apply(this, arguments);
    }
  }

  onFileUploadedAndValid = function (size, name, content_type, file_base64) {
    // this.binary_value = true;
    // this.ngModel = file_base64;
    this.change.emit(file_base64);
  }
}
