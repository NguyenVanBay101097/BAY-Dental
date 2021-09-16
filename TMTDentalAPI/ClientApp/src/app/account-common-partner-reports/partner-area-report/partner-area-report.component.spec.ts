import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerAreaReportComponent } from './partner-area-report.component';

describe('PartnerAreaReportComponent', () => {
  let component: PartnerAreaReportComponent;
  let fixture: ComponentFixture<PartnerAreaReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerAreaReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerAreaReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
