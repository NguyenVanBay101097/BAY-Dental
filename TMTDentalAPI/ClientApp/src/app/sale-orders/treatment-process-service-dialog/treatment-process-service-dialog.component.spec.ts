import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TreatmentProcessServiceDialogComponent } from './treatment-process-service-dialog.component';

describe('TreatmentProcessServiceDialogComponent', () => {
  let component: TreatmentProcessServiceDialogComponent;
  let fixture: ComponentFixture<TreatmentProcessServiceDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TreatmentProcessServiceDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TreatmentProcessServiceDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
