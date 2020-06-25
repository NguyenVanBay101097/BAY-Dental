import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { WebService } from 'src/app/core/services/web.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-image-file-upload',
  templateUrl: './image-file-upload.component.html',
  styleUrls: ['./image-file-upload.component.css']
})
export class ImageFileUploadComponent implements OnInit, OnChanges {

  placeholder = '/assets/images/placeholder.png';
  @Input() imageId: string;
  uploading = false;
  @Output() uploaded = new EventEmitter<any>();

  constructor(private webService: WebService) { }

  ngOnChanges(changes: SimpleChanges): void {
  }

  ngOnInit() {
  }

  onFileChange(e) {
    var file_node = e.target;
    var file = file_node.files[0];

    var formData = new FormData();
    formData.append('file', file);

    this.webService.uploadImage(formData).subscribe((result: any) => {
      this.uploaded.emit(result);
    });
  }

  onClear() {
    this.uploaded.emit(null);
  }

  get imageFileUrl() {
    return environment.uploadDomain + 'api/Web/Image/' + this.imageId;
  }
}
