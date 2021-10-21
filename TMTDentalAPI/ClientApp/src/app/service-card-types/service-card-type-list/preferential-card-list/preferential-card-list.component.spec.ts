import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PreferentialCardListComponent } from './preferential-card-list.component';

describe('PreferentialCardListComponent', () => {
  let component: PreferentialCardListComponent;
  let fixture: ComponentFixture<PreferentialCardListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PreferentialCardListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PreferentialCardListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
