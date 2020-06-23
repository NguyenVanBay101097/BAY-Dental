import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterDropdownComponent } from './audience-filter-dropdown.component';

describe('AudienceFilterDropdownComponent', () => {
  let component: AudienceFilterDropdownComponent;
  let fixture: ComponentFixture<AudienceFilterDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
