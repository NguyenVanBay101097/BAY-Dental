import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookAudienceFilterDropdownComponent } from './facebook-audience-filter-dropdown.component';

describe('FacebookAudienceFilterDropdownComponent', () => {
  let component: FacebookAudienceFilterDropdownComponent;
  let fixture: ComponentFixture<FacebookAudienceFilterDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookAudienceFilterDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookAudienceFilterDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
