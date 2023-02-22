import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SamplePrescriptionLineCreateUpdateDialogComponent } from './sample-prescription-line-create-update-dialog.component';

describe('SamplePrescriptionLineCreateUpdateDialogComponent', () => {
  let component: SamplePrescriptionLineCreateUpdateDialogComponent;
  let fixture: ComponentFixture<SamplePrescriptionLineCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SamplePrescriptionLineCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SamplePrescriptionLineCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
