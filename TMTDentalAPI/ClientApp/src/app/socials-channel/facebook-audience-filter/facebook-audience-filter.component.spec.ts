import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookAudienceFilterComponent } from './facebook-audience-filter.component';

describe('FacebookAudienceFilterComponent', () => {
  let component: FacebookAudienceFilterComponent;
  let fixture: ComponentFixture<FacebookAudienceFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookAudienceFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookAudienceFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
