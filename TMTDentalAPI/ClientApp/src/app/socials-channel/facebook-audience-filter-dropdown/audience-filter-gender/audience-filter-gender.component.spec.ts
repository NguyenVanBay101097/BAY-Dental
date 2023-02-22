import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterGenderComponent } from './audience-filter-gender.component';

describe('AudienceFilterGenderComponent', () => {
  let component: AudienceFilterGenderComponent;
  let fixture: ComponentFixture<AudienceFilterGenderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterGenderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterGenderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
