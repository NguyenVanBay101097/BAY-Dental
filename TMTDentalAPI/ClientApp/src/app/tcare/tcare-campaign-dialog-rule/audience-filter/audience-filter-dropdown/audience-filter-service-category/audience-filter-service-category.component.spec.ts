import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AudienceFilterServiceCategoryComponent } from './audience-filter-service-category.component';

describe('AudienceFilterServiceCategoryComponent', () => {
  let component: AudienceFilterServiceCategoryComponent;
  let fixture: ComponentFixture<AudienceFilterServiceCategoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AudienceFilterServiceCategoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AudienceFilterServiceCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
