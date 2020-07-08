import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToaThuocSamplePrescriptionComponent } from './toa-thuoc-sample-prescription.component';

describe('ToaThuocSamplePrescriptionComponent', () => {
  let component: ToaThuocSamplePrescriptionComponent;
  let fixture: ComponentFixture<ToaThuocSamplePrescriptionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToaThuocSamplePrescriptionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToaThuocSamplePrescriptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
