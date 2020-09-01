import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterLastExaminationComponent } from './audience-filter-last-examination.component';

describe('AudienceFilterLastExaminationComponent', () => {
  let component: AudienceFilterLastExaminationComponent;
  let fixture: ComponentFixture<AudienceFilterLastExaminationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterLastExaminationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterLastExaminationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
