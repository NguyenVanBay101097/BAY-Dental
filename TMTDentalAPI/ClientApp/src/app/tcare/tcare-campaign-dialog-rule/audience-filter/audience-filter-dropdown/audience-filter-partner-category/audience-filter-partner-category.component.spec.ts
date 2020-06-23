import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterPartnerCategoryComponent } from './audience-filter-partner-category.component';

describe('AudienceFilterPartnerCategoryComponent', () => {
  let component: AudienceFilterPartnerCategoryComponent;
  let fixture: ComponentFixture<AudienceFilterPartnerCategoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterPartnerCategoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterPartnerCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
