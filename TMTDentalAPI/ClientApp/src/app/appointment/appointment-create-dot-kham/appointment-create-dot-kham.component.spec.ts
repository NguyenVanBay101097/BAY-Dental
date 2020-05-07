import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentCreateDotKhamComponent } from './appointment-create-dot-kham.component';

describe('AppointmentCreateDotKhamComponent', () => {
  let component: AppointmentCreateDotKhamComponent;
  let fixture: ComponentFixture<AppointmentCreateDotKhamComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppointmentCreateDotKhamComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentCreateDotKhamComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
