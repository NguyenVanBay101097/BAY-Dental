import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SamplePrescriptionCreateUpdateDialogComponent } from './sample-prescription-create-update-dialog.component';

describe('SamplePrescriptionCreateUpdateDialogComponent', () => {
  let component: SamplePrescriptionCreateUpdateDialogComponent;
  let fixture: ComponentFixture<SamplePrescriptionCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SamplePrescriptionCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SamplePrescriptionCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
