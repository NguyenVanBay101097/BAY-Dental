import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-tcare-quickreply-dialog',
  templateUrl: './tcare-quickreply-dialog.component.html',
  styleUrls: ['./tcare-quickreply-dialog.component.css']
})
export class TcareQuickreplyDialogComponent implements OnInit {
  model: any;
  formGroup: FormGroup;
  submited = false;
  title: string;
  showPluginTextarea: boolean = false;
  selectArea_start: number;
  selectArea_end: number;
  textareaLength = 640;

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private intlService: IntlService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({    
      content: ['', Validators.required]     
    });
  }

  get contentControl() {
    return this.formGroup.get('content');
  }

  onSend(){
    this.activeModal.close(this.contentControl);
  }

  selectArea(event) {
    this.selectArea_start = event.target.selectionStart;
    this.selectArea_end = event.target.selectionEnd;
  }

  getLimitText() {
    const text = this.formGroup.get('content').value;
    if (text) {
      return this.textareaLength - text.length;
    } else {
      return this.textareaLength;
    }
  }

  showEmoji() {
    this.showPluginTextarea = true;
  }

  hideEmoji() {
    this.showPluginTextarea = false;
  }

}
