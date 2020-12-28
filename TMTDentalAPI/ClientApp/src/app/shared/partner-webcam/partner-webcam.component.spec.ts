import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerWebcamComponent } from './partner-webcam.component';

describe('PartnerWebcamComponent', () => {
  let component: PartnerWebcamComponent;
  let fixture: ComponentFixture<PartnerWebcamComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerWebcamComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerWebcamComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
