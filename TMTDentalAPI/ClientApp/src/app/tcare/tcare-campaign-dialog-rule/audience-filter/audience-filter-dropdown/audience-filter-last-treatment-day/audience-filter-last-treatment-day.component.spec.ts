import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterLastTreatmentDayComponent } from './audience-filter-last-treatment-day.component';

describe('AudienceFilterLastTreatmentDayComponent', () => {
  let component: AudienceFilterLastTreatmentDayComponent;
  let fixture: ComponentFixture<AudienceFilterLastTreatmentDayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterLastTreatmentDayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterLastTreatmentDayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
