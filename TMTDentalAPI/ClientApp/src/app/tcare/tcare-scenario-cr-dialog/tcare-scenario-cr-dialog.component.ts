import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareService } from '../tcare.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-tcare-scenario-cr-dialog',
  templateUrl: './tcare-scenario-cr-dialog.component.html',
  styleUrls: ['./tcare-scenario-cr-dialog.component.css']
})
export class TcareScenarioCrDialogComponent implements OnInit {

  title: string;
  formGroup: FormGroup;

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
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
    this.tcareService.createScenario(value).subscribe(
      result => {
        this.router.navigateByUrl('tcare-scenario/' + result.id);
        this.activeModal.close(result);
      }
    )
  }
}
