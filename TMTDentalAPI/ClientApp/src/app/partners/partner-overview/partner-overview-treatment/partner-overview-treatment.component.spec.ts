import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewTreatmentComponent } from './partner-overview-treatment.component';

describe('PartnerOverviewTreatmentComponent', () => {
  let component: PartnerOverviewTreatmentComponent;
  let fixture: ComponentFixture<PartnerOverviewTreatmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewTreatmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewTreatmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
