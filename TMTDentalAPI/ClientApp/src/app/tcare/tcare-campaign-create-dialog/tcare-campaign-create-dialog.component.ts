import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareService } from '../tcare.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-tcare-campaign-create-dialog',
  templateUrl: './tcare-campaign-create-dialog.component.html',
  styleUrls: ['./tcare-campaign-create-dialog.component.css']
})
export class TcareCampaignCreateDialogComponent implements OnInit {

  formGroup: FormGroup

  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private tcareService: TcareService,
    private router: Router
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required]
    })
  }

  onSave() {
    if (this.formGroup.invalid) {
      return false;
    }
    var value = this.formGroup.value;
    this.tcareService.nameCreate(value).subscribe(
      result => {
        this.router.navigateByUrl(`tcare/${result.id}`);
        this.activeModal.close();
      }
    )
  }
}
